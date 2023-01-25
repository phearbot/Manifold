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
    [SerializeField] GameObject capsule;
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject gravityReference;
    [SerializeField] GameObject wasdReference;
    Vector3 playerGravityVelocity;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float maxGravityMagnitude = .5f;
    [SerializeField] float groundedRaycastLength;
    bool isGrounded;

    Canvas canvas;
    Image reticle;
    NormalColorMapper mapper;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; 
        controller = GetComponent<CharacterController>();
        canvas = FindObjectOfType<Canvas>();
        reticle = canvas.GetComponentInChildren<Image>();
        mapper = GetComponent<NormalColorMapper>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
	}

	private void FixedUpdate()
	{
        CheckReticle();
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

    void CheckForGrounded()
    {
        // This raycast isn't hitting the feet like it should. Maybe try the maincam position point or something?
		isGrounded = Physics.Raycast(transform.position, -wasdReference.transform.up, out RaycastHit hitInfo, groundedRaycastLength);
        print("isGrounded: " + isGrounded + "; velocity sqr mag: " + controller.velocity.sqrMagnitude);
	}

    void CheckReticle()
    {
        float reticleCheckDistance = 2f;
        bool castHit = Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hitInfo, reticleCheckDistance);

        if (castHit)
        {
            reticle.color = mapper.MapNormalToColor(hitInfo.normal);
        }
		else
            reticle.color = Color.white;
        

    }

	[ContextMenu("Flip Player")]
	void FlipPlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(180,0,0));
    }
}
