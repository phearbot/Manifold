using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] GameObject capsule;
    [SerializeField] Camera mainCam;
    Rigidbody rb;
    Vector3 playerVelocity;
    private Vector2 turn;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float sensitivityX = 1.0f;
	[SerializeField] float sensitivityY = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; 
        controller = GetComponent<CharacterController>();
        rb = capsule.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		turn.x += Input.GetAxis("Mouse X");
		turn.y += Input.GetAxis("Mouse Y");
		transform.localRotation = Quaternion.Euler(0, turn.x * sensitivityX, 0);
		mainCam.transform.localRotation = Quaternion.Euler(-turn.y * sensitivityY, 0, 0);

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move(move * 5f * moveSpeed * Time.deltaTime);
	}

    private void FixedUpdate()
    {


        //ector3 moveDir = new Vector3(ver, 0, hor);
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        

    
    }
}
