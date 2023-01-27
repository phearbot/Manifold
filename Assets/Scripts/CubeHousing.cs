using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHousing : MonoBehaviour
{
	[SerializeField] Cube activationCube;
    [SerializeField] Door[] doors;

    bool cubeAtFinalPosition = false;
    [SerializeField] AudioSource humStart;
	[SerializeField] AudioSource humLoop;
    AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void FixedUpdate()
	{
        if (activationCube != null &! cubeAtFinalPosition)
        {
			Vector3 direction = transform.position - activationCube.transform.position;
			activationCube.rb.velocity = direction * 2.5f;

            // Lock the cube in place when it's close enough
            float val = Vector3.SqrMagnitude(transform.position - activationCube.rb.position);
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

        foreach (Door door in doors)
        {
            door.OpenDoor();
        }

		am.Play("CubePlace1");
		humStart.Play();
        humLoop.PlayDelayed(humStart.clip.length);
    }

    public void Deactivate()
    {
        activationCube = null;
        cubeAtFinalPosition = false;

		foreach (Door door in doors)
		{
			door.CloseDoor();
		}

		humStart.Stop();
        humLoop.Stop();
	}
}
