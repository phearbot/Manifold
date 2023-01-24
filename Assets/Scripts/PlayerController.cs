using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; 
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        wasdReference.transform.rotation = Quaternion.Euler(0, mainCam.transform.localEulerAngles.y, 0);
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
            playerGravityVelocity += (transform.up * gravity * Time.deltaTime);
			playerGravityVelocity = new Vector3(0, Mathf.Clamp(playerGravityVelocity.y, -maxGravitySpeed, maxGravitySpeed));
            //print("playerVelocity: " + playerGravityVelocity + "; controller velocity: " + controller.velocity);
		}

        // Apply movement on local X/Z axes
        controller.Move(move * 5f * moveSpeed * Time.deltaTime + playerGravityVelocity);
        //controller.velocity = new Vector3(controller.velocity.x, Mathf.Clamp(controller.velocity.y, -maxGravitySpeed, maxGravitySpeed), controller.velocity.z);
            
	}

	[ContextMenu("Flip Player")]
	void FlipPlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(180,0,0));
    }
}
