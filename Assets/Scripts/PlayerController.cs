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
    CharacterController controller;
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject gravityReference;
    [SerializeField] GameObject wasdReference;
    AudioManager am;
	Canvas canvas;

	[Header("Movement Variables")]
    
	Vector3 playerGravityVelocity;
	[SerializeField] float moveSpeed = 1f;
    [SerializeField] float sprintModifier = 1.2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float maxGravityMagnitude = .3f;
    [SerializeField] float groundedRaycastLength;
    bool isGrounded;
	bool isMoving; // sound hook


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

    // Interactable Variables
    [SerializeField] Transform carryPoint;
    bool reticleHasInteractableLock = false;
    GameObject interactableTarget;
    Sprite canInteractSprite;
    Sprite isInteractingSprite;
    Cube cubeBeingCarried;

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false; 
        controller = GetComponent<CharacterController>();

        canvas = FindObjectOfType<Canvas>();
        reticle = canvas.GetComponentInChildren<Image>();
        interactableHex = GameObject.Find("InteractableHex").GetComponent<Image>();
        interactableHex.enabled = false;
        canInteractSprite = Resources.Load<Sprite>("Art/Sprites/HexagonDouble");
        isInteractingSprite = Resources.Load<Sprite>("Art/Sprites/HexagonSingle");
        mapper = GetComponent<NormalColorMapper>();
        LockAndUnlockCubes();

		Material normalColorMat = Resources.Load("Art/Shaders and Materials/Normal Vector Material") as Material;
		normalColorMat.SetVector("_TargetNormal", Vector3.up);

		am = FindObjectOfType<AudioManager>();
		am.PlayNoRestartIfPlaying("BGM");
	}

    // Update is called once per frame
    void Update()
    {
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
				PickupObject();
			}
            else if (cubeBeingCarried != null) 
            {
				DropObject();
            }


        }
    }

    void ChangeGravityField()
    {
		Quaternion rotation = Quaternion.FromToRotation(transform.up, targetNormal);

		print("transform.up: " + transform.up + "; targetNormal: " + targetNormal + "; rotation: " + rotation.eulerAngles);
		previousGravityRotation = transform.rotation;
		nextGravityTargetRotation = rotation * transform.rotation;


		currentlyChangingGravity = true;
		gravityRotationTimer = 0;

        // Update the material for shader
        Material normalColorMat = Resources.Load("Art/Shaders and Materials/Normal Vector Material") as Material;
        normalColorMat.SetVector("_TargetNormal", targetNormal);

        am.Play(mapper.MapNormalToSFX(targetNormal));
	}

    void LockAndUnlockCubes()
    {
        foreach (Cube cube in FindObjectsOfType<Cube>())
        {
            if (cube.transform.up == transform.up)
                cube.UnlockCube();
            else
                cube.LockCube();
        }
    }

    void PickupObject()
    {
        am.Play("CubePickup");
		SwapInteractingSprite(true);
		cubeBeingCarried = interactableTarget.GetComponent<Cube>();
        cubeBeingCarried.GetCarried(carryPoint);
    }

    public void DropObject()
    {
        am.Play("CubeDrop");
		SwapInteractingSprite(false);
		cubeBeingCarried.GetDropped();
        cubeBeingCarried = null;
    }

    void CheckForGrounded()
    {
        // This raycast isn't hitting the feet like it should. Maybe try the maincam position point or something?
		isGrounded = Physics.Raycast(transform.position, -wasdReference.transform.up, out RaycastHit hitInfo, groundedRaycastLength);
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
        am.PlayNoRestartIfPlaying("Footstep Run");
    }

    void StopWalkSound()
    {
        am.Stop("Footstep Step");
        am.Stop("Footstep Walk");
        am.Stop("Footstep Run");
    }
}
