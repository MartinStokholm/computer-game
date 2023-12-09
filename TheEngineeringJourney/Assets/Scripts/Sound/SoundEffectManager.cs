using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int soundsVolume = 8;

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }
       
        SetSoundsVolume(soundsVolume);
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    /// <summary>
    /// Play the sound effect
    /// </summary>
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // Play sound using a sound gameobject and component from the object pool
        var sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.SoundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.SoundEffectClip.length));
    }


    /// <summary>
    /// Disable sound effect object after it has played thus returning it to the object pool
    /// </summary>
    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    //
    /// <summary>
    /// Increase sounds volume
    /// </summary>
    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (soundsVolume >= maxSoundsVolume) return;

        ++soundsVolume;

        SetSoundsVolume(soundsVolume); ;
    }

    /// <summary>
    /// Decrease sounds volume
    /// </summary>
    public void DecreaseSoundsVolume()
    {
        if (soundsVolume == 0) return;

        --soundsVolume;

        SetSoundsVolume(soundsVolume);
    }

    /// <summary>
    /// Set sounds volume
    /// </summary>
    private void SetSoundsVolume(int soundsVolume)
    {
        var muteDecibels = -80f;

        GameResources.Instance.SoundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume",
            soundsVolume == 0 ? muteDecibels : MusicHelper.LinearToDecibels(soundsVolume));
    }
}
