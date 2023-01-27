using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{

    public float BGMFadeTimer = 2f;

    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            DestroyImmediate(gameObject); // This had to be done because the Player script was finding the destroyed one and throwing a null reference
            //Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

	public void PlayNoRestartIfPlaying(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		if (!s.source.isPlaying)
			s.source.Play();

	}

    /// <summary>
    /// Name clips in the audio manager [name]Start and [name]Loop. Make sure the loop box is ticked on the loop.
    /// </summary>
    /// <param name="name"></param>
    public void PlayLoop(string name)
    {
		Sound s = Array.Find(sounds, sound => sound.name == name + "Start");
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + "Start not found!");
			return;
		}

		Sound sLoop = Array.Find(sounds, sound => sound.name == name + "Loop");
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + "Loop not found!");
			return;
		}

        s.source.Play();
        sLoop.source.PlayDelayed(s.clip.length);
	}

	public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void FadeoutBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        StartCoroutine(FadeOutBGM(name));
    }

    IEnumerator FadeOutBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = s.volume;
        float currentTime = 0;
        while (s.source.volume > 0)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(s.volume, 0, (currentTime / BGMFadeTimer));
            yield return null;
        }
        s.source.volume = 0;
        s.source.Stop();
        StopCoroutine(FadeOutBGM(name));
    }

    public void FadeinBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        StartCoroutine(FadeInBGM(name));
    }

    IEnumerator FadeInBGM(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
        s.source.volume = 0;
        float currentTime = 0;
        while (s.source.volume < s.volume)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(0, s.volume, (currentTime / BGMFadeTimer));
            yield return null;
        }
        s.source.volume = s.volume;
        StopCoroutine(FadeInBGM(name));
    }


}