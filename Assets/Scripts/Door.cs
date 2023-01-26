using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator[] anims; 

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
        }
    }

    public void CloseDoor()
    {
		foreach (Animator anim in anims)
		{
			anim.SetBool("isOpen", false);
		}
	}
}
