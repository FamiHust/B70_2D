using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool loop;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private List<Sound> sounds = new List<Sound>();
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(s.clip, s.volume);
        }
        else
        {
            Debug.LogWarning("SFX Source is not assigned!");
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        if (musicSource != null)
        {
            musicSource.clip = s.clip;
            musicSource.volume = s.volume;
            musicSource.loop = s.loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music Source is not assigned!");
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}
