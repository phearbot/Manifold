using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButton : MonoBehaviour
{
    AudioManager am;
    [SerializeField] bool isPressed = false;
    [SerializeField] Animator anim;
    [SerializeField] Door door;

    // Start is called before the first frame update
    void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressButton()
    {
        am.Play("UIClick1");
        isPressed = !isPressed;
        anim.SetBool("isPressed", isPressed);

        if (isPressed)
            door.OpenDoor();
        else
            door.CloseDoor();
    }
}
