using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioManager am;
    [SerializeField] GameObject cameraRig;
    [SerializeField] Transform moveTarget;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] Image mainMenuBackground;
    
    // Start is called before the first frame update
    void Start()
    {
        am.FadeinBGM("BGM");
    }

    // Update is called once per frame
    void Update()
    {
        cameraRig.transform.position = Vector3.Lerp(cameraRig.transform.position, moveTarget.position, moveSpeed / 20);
    }
}
