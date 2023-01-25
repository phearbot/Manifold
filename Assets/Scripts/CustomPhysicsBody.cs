using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomPhysicsBody : MonoBehaviour
{
    public bool gravityEnabled;
    public bool snapToGrid = true;
    [SerializeField] float gravityForce = 20;
    [SerializeField] float maxVelocitySqrMagnitude = 10000;

    // References
    Rigidbody rb;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {

	}

	private void FixedUpdate()
	{
        if (gravityEnabled)
        {
            if (rb.velocity.sqrMagnitude < maxVelocitySqrMagnitude) 
				rb.AddForce(-transform.up * gravityForce);

            if (snapToGrid)
                SnapToGrid();
		}
	}

    void SnapToGrid()
    {
        // This needs to be re-written to work on all axes
		if (transform.position.x % .25 != 0 || transform.position.z % .25 != 0)
		{
            //print("should be snapping");
			float newX = Mathf.Round(transform.position.x * 4) / 4;
			float newZ = Mathf.Round(transform.position.z * 4) / 4;

			transform.position = new Vector3(newX, transform.position.y, newZ);
		}

        snapToGrid = false;
	}

}
