using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonobehaviour<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        // Load components
        musicAudioSource = GetComponent<AudioSource>();

        // Start with music off
        GameResources.Instance.MusicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }
    }
    
    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.MusicFadeOutTime, float fadeInTime = Settings.MusicFadeInTime)
    {
        // Play music track
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }
    

    /// <summary>
    /// Play music for room routine
    /// </summary>
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        // if fade out routine already running then stop it
        if (fadeOutMusicCoroutine is { })
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        // if fade in routine already running then stop it
        if (fadeInMusicCoroutine is { })
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // If the music track has changed then play new music track
        if (musicTrack.MusicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.MusicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }
    
    /// Fade out music routine
    /// </summary>
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.MusicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    /// <summary>
    /// Fade in music routine
    /// </summary>
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        // Set clip & play
        musicAudioSource.clip = musicTrack.MusicClip;
        musicAudioSource.volume = musicTrack.MusicVolume;
        musicAudioSource.Play();

        GameResources.Instance.MusicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }
    
    /// <summary>
    /// Increase music volume
    /// </summary>
    public void IncreaseMusicVolume()
    {
        const int maxMusicVolume = 20;

        if (musicVolume >= maxMusicVolume) return;

        ++musicVolume;
        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// Decrease music volume
    /// </summary>
    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        --musicVolume;
        SetMusicVolume(musicVolume);
    }

    public void SetMusicVolume(int musicVolume)
    {
        const float muteDecibels = -80f;

        GameResources.Instance.MusicMasterMixerGroup.audioMixer.SetFloat("musicVolume", 
            musicVolume is 0 
                ? muteDecibels 
                : MusicHelper.LinearToDecibels(musicVolume));
    }
}
