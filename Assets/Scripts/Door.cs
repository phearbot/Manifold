using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator[] anims;
    [SerializeField] AudioSource doorOpen;
	[SerializeField] AudioSource doorClose;

    // Start is called before the first frame update
    void Start()
    {
        anims = GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        foreach (Animator anim in anims)
        {
            anim.SetBool("isOpen", true);
            doorOpen.Play();
        }
    }

    public void CloseDoor()
    {
		foreach (Animator anim in anims)
		{
			anim.SetBool("isOpen", false);
            doorClose.PlayDelayed(.1f); // Delaying this because of the specific clip, may want to remove if different.
		}
	}
}
