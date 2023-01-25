using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHousing : MonoBehaviour
{
	[SerializeField] Cube activationCube;
    [SerializeField] float cubeGrapLerpTime = 1f;
    bool cubeAtFinalPosition = false;

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
        // This doesn't need to run forever CLEANUP (lerp with a timer probs)
        if (activationCube != null &! cubeAtFinalPosition)
        {
			Vector3 direction = transform.position - activationCube.transform.position;
			activationCube.rb.velocity = direction * 2.5f;


            float val = Vector3.SqrMagnitude(transform.position - activationCube.rb.position);
            print(val);

            if (val < .00001f)
            {
                cubeAtFinalPosition = true;
                activationCube.rb.velocity = Vector3.zero;
                activationCube.rb.MovePosition(transform.position);
            }
		}

	}

	public void Activate(Cube _activationCube)
    {
        activationCube = _activationCube;
        cubeAtFinalPosition = false;
    }

    public void Deactivate()
    {
        activationCube = null;
        cubeAtFinalPosition = false;
    }
}
