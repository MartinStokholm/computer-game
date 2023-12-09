using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource.clip is null) return;

        audioSource.Play();
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Set the sound effect to play 
    /// </summary>
    public void SetSound(SoundEffectSO soundEffect)
    {
        audioSource.pitch = Random.Range(soundEffect.SoundEffectPitchRandomVariationMin, soundEffect.SoundEffectPitchRandomVariationMax);
        audioSource.volume = soundEffect.SoundEffectVolume;
        audioSource.clip = soundEffect.SoundEffectClip;
    }
}
