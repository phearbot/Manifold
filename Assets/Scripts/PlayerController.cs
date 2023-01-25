using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject gravityReference;
    [SerializeField] GameObject wasdReference;
    Vector3 playerGravityVelocity;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float maxGravityMagnitude = .3f;
    [SerializeField] float groundedRaycastLength;
    bool isGrounded;

    Canvas canvas;
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
        print("canInteract: " + canInteractSprite);
        isInteractingSprite = Resources.Load<Sprite>("Art/Sprites/HexagonSingle");
        mapper = GetComponent<NormalColorMapper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyChangingGravity)
            HandleGravityRotation();
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
            currentlyChangingGravity = false;
	}

	void HandleMovement()
    {
        CheckForGrounded();

		wasdReference.transform.localRotation = Quaternion.Euler(0, mainCam.transform.localEulerAngles.y, 0);
		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");
		Vector3 move = (mainCam.transform.right * x + wasdReference.transform.forward * z).normalized;

        // print(controller.isGrounded);

        // Only apply gravity is player is not grounded
		if (isGrounded)
			playerGravityVelocity = transform.up * gravity * Time.deltaTime;
		else if (controller.velocity.sqrMagnitude < maxGravityMagnitude)
			playerGravityVelocity += transform.up * gravity * Time.deltaTime;


		// Apply movement on local X/Z axes
		controller.Move(move * 5f * moveSpeed * Time.deltaTime + playerGravityVelocity);

	}

    void HandleOtherInput()
    {
        // Logic for changing gravity
        bool receivedGravityShiftInput = (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space));
        if (receivedGravityShiftInput && reticleHasColorLock)
        {
            Quaternion rotation = Quaternion.FromToRotation(transform.up, targetNormal);

			//print("transform.up: " + transform.up + "; targetNormal: " + targetNormal + "; rotation: " + rotation.eulerAngles);
            previousGravityRotation = transform.rotation;
            nextGravityTargetRotation = rotation * transform.rotation;


            currentlyChangingGravity = true;
            gravityRotationTimer = 0;
        }

        // Logic for picking up / pushing buttons
        if (Input.GetMouseButtonDown(0)) 
        {
            if (interactableTarget != null && cubeBeingCarried == null)
            {
				SwapInteractingSprite();
				PickupObject();
			}
            else if (cubeBeingCarried != null) 
            {
                DropObject();
            }


        }
    }

    void PickupObject()
    {
        if (interactableTarget.GetComponent<Cube>() != null)
        {
            cubeBeingCarried = interactableTarget.GetComponent<Cube>();
            cubeBeingCarried.GetCarried(carryPoint);
        }
    }

    void DropObject()
    {
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
            print(hitInfo.normal + " : " + hitInfo.transform.gameObject);


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
            reticleHasInteractableLock = false;
            interactableTarget = null;
		}

        // Ideally this wouldn't be called every frame
        interactableHex.enabled = reticleHasInteractableLock;
	}

	[ContextMenu("Flip Player")]
	void FlipPlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(180,0,0));
    }


    void SwapInteractingSprite()
    {
        //print(canInteractSprite);

        if (interactableHex.sprite == canInteractSprite)
            interactableHex.sprite = isInteractingSprite;
        else
            interactableHex.sprite = canInteractSprite;
    }
}
