using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float maxGravitySpeed = .5f;

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
		wasdReference.transform.localRotation = Quaternion.Euler(0, mainCam.transform.localEulerAngles.y, 0);
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
		Vector3 move = mainCam.transform.right * x + wasdReference.transform.forward * z;

		if (controller.isGrounded)
		{
			playerGravityVelocity = Vector3.zero;
		}
		else
		{
            // Increment Velocity and clamp it to max speed (up or down)
            //print(transform.up);
			playerGravityVelocity +=  Vector3.ClampMagnitude((transform.up * gravity * Time.deltaTime), maxGravitySpeed);
            print(playerGravityVelocity);
			//playerGravityVelocity = new Vector3(0, Mathf.Clamp(playerGravityVelocity.y, -maxGravitySpeed, maxGravitySpeed)); // This clamp assumes gravity is always Y axis
		}

		// Apply movement on local X/Z axes
		controller.Move(move * 5f * moveSpeed * Time.deltaTime + playerGravityVelocity);
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
