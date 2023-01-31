using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Material colorByNormalMat;

    [Header("Camera Variables")]
    [SerializeField] GameObject cameraRig;
    [SerializeField] Transform moveTarget;
    [SerializeField] float moveSpeed = 1f;

    [Header("Text and Animations")]
    [SerializeField] Image mainMenuBackground;
    [SerializeField] TextMeshProUGUI manifoldGarden;
    [SerializeField] float manifoldGardenTime;
	[SerializeField] TextMeshProUGUI pressAnyButton;
    [SerializeField] float pressAnyButtonTime;
    [SerializeField] float flickerSpeed;
    float animationTimer;
    bool passedIntroScreen;



	// Start is called before the first frame update
	void Start()
    {
        AudioManager.instance.FadeinBGM("BGM");

        animationTimer = 0;
        passedIntroScreen = false;

        // Set the color for the menu
        colorByNormalMat.SetVector("_TargetNormal", Vector3.back);

    }

    // Update is called once per frame
    void Update()
    {
        // Moves the camera rig in the menu
        cameraRig.transform.position = Vector3.Lerp(cameraRig.transform.position, moveTarget.position, moveSpeed * Time.deltaTime);

        if (animationTimer < 10)
			HandleAnimations();

        if (Input.anyKeyDown &! passedIntroScreen && animationTimer > manifoldGardenTime + 1f)
        {
            passedIntroScreen = true;
            manifoldGarden.GetComponent<Animator>().SetBool("ShowText", false);
            pressAnyButton.GetComponent<Animator>().SetBool("ShowText", false);
            mainMenuBackground.GetComponent<Animator>().SetTrigger("PressedAnyButton");
			StartCoroutine(StartGame());
		}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    void HandleAnimations()
    {
		animationTimer += Time.deltaTime;

		if (animationTimer > manifoldGardenTime) 
        {
            manifoldGarden.GetComponent<Animator>().SetTrigger("EnableText");
        }

        if (animationTimer > pressAnyButtonTime)
        {
            pressAnyButton.GetComponent<Animator>().SetTrigger("EnableText");
        }
	}

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
