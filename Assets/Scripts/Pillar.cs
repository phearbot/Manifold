using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] public bool pillarIsActivated;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivatePillar()
    {
        pillarIsActivated = true;
        anim.SetBool("Activated", true);
        AudioManager.instance.Play("PillarActivate");
        GameManager.instance.NotifyOfPillarStateChange();
    }

    public void DeactivatePillar()
    {
        pillarIsActivated = false;
        anim.SetBool("Activated", false);
        AudioManager.instance.Play("PillarDeactivate");
		GameManager.instance.NotifyOfPillarStateChange();
	}
}
