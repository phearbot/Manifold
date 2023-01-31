using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	[SerializeField] Pillar[] pillars;
	[SerializeField] int activePillars;
	[SerializeField] Animator fadeToBlack;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
		{
			Destroy(gameObject);
										  
			return;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		// pillars = FindObjectsOfType<Pillar>();
	}

	public void AddPillarsToArray()
	{
		pillars = FindObjectsOfType<Pillar>();
	}

	public void NotifyOfPillarStateChange()
	{
		activePillars = 0;

		foreach (Pillar pillar in pillars)
		{
			if (pillar.pillarIsActivated)
				activePillars++;
		}

		if (activePillars == 4)
			WinGame();
	}

	[ContextMenu("Win Game")]
	public void WinGame()
	{
		Debug.Log("GG gamer.");
		fadeToBlack.SetTrigger("WinGame");

		StartCoroutine(LoadCredits());
	}

	IEnumerator LoadCredits()
	{
		yield return new WaitForSeconds(4f);

		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
