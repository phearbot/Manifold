using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    Transform carryPoint;
    CustomPhysicsBody cpb;
    public bool isBeingCarried = false;
    public Rigidbody rb;
    public CubeHousing cubeHousing;
    [SerializeField] AudioSource thunk;

    [Header("Grounded Variables")]
    [SerializeField] bool isGrounded;
    [SerializeField] float castDistance;
    [SerializeField] float boxSize;
    [SerializeField] float fallDurationForSound = .1f;
    float fallTimer;




	private void Awake()
	{
		cpb = GetComponent<CustomPhysicsBody>();
		rb = GetComponent<Rigidbody>();
	}

	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void FixedUpdate()
	{
		if (isBeingCarried)
        {
            Vector3 direction = carryPoint.position - rb.position;
            rb.velocity = direction * 25;
        }

        CheckForGrounded();
	}

    void CheckForGrounded()
    {
        //print("transform up: " + transform.up);
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * boxSize / 2, -transform.up, Quaternion.identity, castDistance);

        // Sets grounded to false and only changes it when it's true each frame
        isGrounded = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag != "Interactable")
            { 
				isGrounded = true;
			}
        }

        if (isGrounded)
        {
            if (fallTimer > fallDurationForSound)
                thunk.Play();

            fallTimer = 0;
        }
        else
			fallTimer += Time.deltaTime;


	}

	public void GetCarried(Transform _carryPoint)
    {
        isBeingCarried = true;
        carryPoint = _carryPoint;
        cpb.gravityEnabled = false;
        cpb.snapToGrid = false;

		if (cubeHousing != null)
			cubeHousing.Deactivate();
	}

    public void GetDropped()
    {
        isBeingCarried = false;
        carryPoint = null;
        cpb.gravityEnabled = true;
        cpb.snapToGrid = true;
        rb.velocity = Vector3.zero;

    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Cube Housing")
        {
            if (isBeingCarried)
                FindObjectOfType<PlayerController>().DropObject();

            cubeHousing = other.GetComponent<CubeHousing>();
            cubeHousing.Activate(this);
            cpb.gravityEnabled = false;

        }
	}

    public void LockCube()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnlockCube()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
	}
}
