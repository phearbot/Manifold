using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using System;



public class PlayerController : MonoBehaviour
{
    [Header("Objects From the Scene")]
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject gravityReference;
    [SerializeField] GameObject wasdReference;
	Canvas canvas;
    Transform spawnPoint;

	[Header("Movement Variables")]
	[SerializeField] CharacterController controller;
	[SerializeField] float moveSpeed = 1f;
    [SerializeField] float sprintModifier = 1.2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float maxGravityMagnitude = .3f;
    [SerializeField] float groundedRaycastLength;
    [SerializeField] bool isGrounded;
	Vector3 playerGravityVelocity;
	bool isMoving; // sound hook
    float fallTimer;
    [SerializeField] float fallTimeSoundActivation;


	Image reticle;
    Image interactableHex;
    NormalColorMapper mapper;


	// Gravity Rotation Variables
	[SerializeField] float reticleCheckDistance = 3f;
	Vector3 targetNormal;
    bool reticleHasColorLock = false;
    bool currentlyChangingGravity = false;
    Quaternion previousGravityRotation;
    Quaternion nextGravityTargetRotation;
    [SerializeField] float gravityRotationDuration = .5f;
    float gravityRotationTimer;
    Material normalColorMat;

	// Interactable Variables
	[SerializeField] Transform carryPoint;
    bool reticleHasInteractableLock = false;
    GameObject interactableTarget;
    Sprite canInteractSprite;
    Sprite isInteractingSprite;
    Cube cubeBeingCarried;

	private void Awake()
	{
		spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
		//Debug.Log("transform.position before: " + transform.position + "; spawn point position: " + spawnPoint.position);
        //transform.position = spawnPoint.position;

        controller.Move(spawnPoint.position);
        transform.rotation = spawnPoint.rotation;

		//Debug.Log("transform.position after: " + transform.position);
		//Debug.Log("Purple treehouse transform position: " + GameObject.FindGameObjectWithTag("Finish").transform.position);
	}

	// Start is called before the first frame update
	void Start()
    {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false; 
        // controller = GetComponent<CharacterController>();

        canvas = FindObjectOfType<Canvas>();
        reticle = canvas.GetComponentInChildren<Image>();
        interactableHex = GameObject.Find("InteractableHex").GetComponent<Image>();
        interactableHex.enabled = false;
        canInteractSprite = Resources.Load<Sprite>("Art/Sprites/HexagonDouble");
        isInteractingSprite = Resources.Load<Sprite>("Art/Sprites/HexagonSingle");
        mapper = GetComponent<NormalColorMapper>();
        LockAndUnlockCubes();

        normalColorMat = Resources.Load("Art/Shaders and Materials/Color By Normal") as Material;
		mapper.MapMaterialColors(normalColorMat);
		normalColorMat.SetVector("_TargetNormal", transform.up);


        AudioManager.instance.BGMFadeTimer = 10f;
        AudioManager.instance.FadeoutBGM("BGM");




	}

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.time + " : " + transform.position);

        if (currentlyChangingGravity)
        {
            StopWalkSound();
			HandleGravityRotation();
		}
        else
        {
			HandleMovement();
			HandleOtherInput();
		}

	}

	private void FixedUpdate()
	{
        CheckReticle();
	}

    void HandleGravityRotation()
    {
        gravityRotationTimer += Time.deltaTime;
		transform.rotation = Quaternion.Lerp(previousGravityRotation, nextGravityTargetRotation, gravityRotationTimer / gravityRotationDuration);

        if (gravityRotationTimer > gravityRotationDuration)
        {
			currentlyChangingGravity = false;
			LockAndUnlockCubes();
		}
	}

	void HandleMovement()
    {
        CheckForGrounded();

		wasdReference.transform.localRotation = Quaternion.Euler(0, mainCam.transform.localEulerAngles.y, 0);
		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical") * 2; // this makes the forward / back movement relatively faster, not loving it but it's ok
		Vector3 move = (mainCam.transform.right * x + wasdReference.transform.forward * z).normalized;

        // This shouldn't need to happen every frame
        if (move.sqrMagnitude > 0 && isGrounded)
            PlayWalkSound();
        else
            StopWalkSound();

        // Only apply gravity is player is not grounded
		if (isGrounded)
			playerGravityVelocity = transform.up * gravity * Time.deltaTime;
		else if (controller.velocity.sqrMagnitude < maxGravityMagnitude)
			playerGravityVelocity += transform.up * gravity * Time.deltaTime;



        float _sprint = Input.GetKey(KeyCode.LeftShift) ? sprintModifier : 1;

		// Apply movement on local X/Z axes
		controller.Move(move * 5f * moveSpeed * _sprint * Time.deltaTime + playerGravityVelocity);

	}

    void HandleOtherInput()
    {
        // Logic for changing gravity
        bool receivedGravityShiftInput = (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space));
        if (receivedGravityShiftInput && reticleHasColorLock && cubeBeingCarried == null && transform.up != targetNormal)
        {
            ChangeGravityField();
        }

        // Logic for picking up / pushing buttons
        if (Input.GetMouseButtonDown(0)) 
        {
            // the third scenario below may be replaced by a bool on the object at some point
            if (interactableTarget != null && cubeBeingCarried == null && interactableTarget.transform.up == transform.up)
            {
                if (interactableTarget.GetComponent<Cube>() != null)
                    PickupObject();
                else if
                    (interactableTarget.GetComponent<CubeSpawner>() != null)
                    RespawnCubeAndPickup();
                else if (interactableTarget.transform.parent.GetComponent<SwitchButton>() != null) // hacky, clean this up and in the pressbutton function
                    PressButton();
			}
            else if (cubeBeingCarried != null) 
            {
				DropObject();
            }


        }
    }

    void RespawnCubeAndPickup()
    {
		AudioManager.instance.Play("CubePickup");
		SwapInteractingSprite(true);
		cubeBeingCarried = interactableTarget.GetComponent<CubeSpawner>().SpawnNewCube();
		cubeBeingCarried.GetCarried(carryPoint);
	}

    void PressButton()
    {
        SwitchButton button = interactableTarget.transform.parent.GetComponent<SwitchButton>();
        button.PressButton();
        
    }

    void ChangeGravityField()
    {
		Quaternion rotation = Quaternion.FromToRotation(transform.up, targetNormal);

		//print("transform.up: " + transform.up + "; targetNormal: " + targetNormal + "; rotation: " + rotation.eulerAngles);
		previousGravityRotation = transform.rotation;
		nextGravityTargetRotation = rotation * transform.rotation;


		currentlyChangingGravity = true;
		gravityRotationTimer = 0;
        playerGravityVelocity = Vector3.zero;
        // controller.velocity = Vector3.zero; can't do this so need to stop my gravity

        // Update the material for shader
        // Material normalColorMat = Resources.Load("Art/Shaders and Materials/Color By Normal") as Material;
        normalColorMat.SetVector("_TargetNormal", targetNormal);

		AudioManager.instance.Play(mapper.MapNormalToSFX(targetNormal));
		AudioManager.instance.Stop("Wind");
	}

    void LockAndUnlockCubes()
    {
        foreach (Cube cube in FindObjectsOfType<Cube>())
        {
            if (cube.transform.up == transform.up &! cube.onTree)
                cube.UnlockCube();
            else
                cube.LockCube();
        }
    }

    void PickupObject()
    {
		AudioManager.instance.Play("CubePickup");
		SwapInteractingSprite(true);
		cubeBeingCarried = interactableTarget.GetComponent<Cube>();
        cubeBeingCarried.GetCarried(carryPoint);
    }

    public void DropObject()
    {
		AudioManager.instance.Play("CubeDrop");
		SwapInteractingSprite(false);
		cubeBeingCarried.GetDropped();
        cubeBeingCarried = null;
    }

    void CheckForGrounded()
    {
        // This raycast isn't hitting the feet like it should. Maybe try the maincam position point or something?
        //isGrounded = Physics.Raycast(transform.position, -wasdReference.transform.up, out RaycastHit hitInfo, groundedRaycastLength);\
        
        isGrounded = Physics.SphereCast(transform.position, controller.radius, -wasdReference.transform.up, out RaycastHit hitInfo, groundedRaycastLength);
        

        if (isGrounded)
        {

			if (fallTimer > 0)
            {
                if (hitInfo.transform.tag == "Glass")
					AudioManager.instance.Play("GlassThud");

				AudioManager.instance.Play("PlayerThud");
			}

			fallTimer = 0;
			AudioManager.instance.Stop("Wind");
		}
        else
            fallTimer += Time.deltaTime;

        if (fallTimer > fallTimeSoundActivation)
			AudioManager.instance.PlayNoRestartIfPlaying("Wind");
        //print("isGrounded: " + isGrounded + "; velocity sqr mag: " + controller.velocity.sqrMagnitude);
	}

    void CheckReticle()
    {
        
        int mask =~ LayerMask.GetMask("Player"); // the tilda inverts the mask so the raycast will not hit the player
		bool castHit = Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hitInfo, reticleCheckDistance, mask);


        

        if (castHit)
        {
            // values for gravity shifting / color stuff
            reticle.color = mapper.MapNormalToColor(hitInfo.normal);
            targetNormal = hitInfo.normal;
            reticleHasColorLock = true;
            //print(hitInfo.normal + " : " + hitInfo.transform.gameObject);


            // logic for interactables
            if (hitInfo.transform.tag == "Interactable")
            {
                reticleHasInteractableLock = true;
                interactableTarget = hitInfo.transform.gameObject;
            }
            else
            {
                reticleHasInteractableLock = false;
                interactableTarget = null;
            }

        }
		else
        {
			reticle.color = Color.white;
            reticleHasColorLock = false;
            interactableTarget = null;
			reticleHasInteractableLock = false;
		}

        // This ensures that the hex doesn't turn off while carrying if your cursor is no longer on it.
        interactableHex.enabled = reticleHasInteractableLock || cubeBeingCarried != null;
	}

	[ContextMenu("Flip Player")]
	void FlipPlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(180,0,0));
    }


    
    void SwapInteractingSprite(bool interacting)
    {
        interactableHex.sprite = interacting ? isInteractingSprite : canInteractSprite;
    }

    void PlayWalkSound()
    {
		AudioManager.instance.PlayNoRestartIfPlaying("Footstep Run");
    }

    void StopWalkSound()
    {
		AudioManager.instance.Stop("Footstep Step");
		AudioManager.instance.Stop("Footstep Walk");
		AudioManager.instance.Stop("Footstep Run");
    }
}
