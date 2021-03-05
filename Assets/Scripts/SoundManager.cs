using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum Sound 
    {
        StartMenuBackground,
        GameBackground,
        BtnHover,
        BtnPress,
        BtnClick,

        Placeholder
    }

    private static Dictionary<Sound, float> soundTimeDictionary;

    private static Dictionary<Sound, GameObject> loopingSounds;

    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimeDictionary = new Dictionary<Sound, float>();
        soundTimeDictionary[Sound.Placeholder] = 0;

        loopingSounds = new Dictionary<Sound, GameObject>();
        loopingSounds[Sound.StartMenuBackground] = null;
        loopingSounds[Sound.GameBackground] = null;
    }

    public static void PlaySound(Sound sound, Vector3 position, bool loop)
    {
        if (!CanPlaySound(sound))
            return;

        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.position = position;
        AudioSource audioSource= soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);

        if(loop && loopingSounds.ContainsKey(sound))
        {
            if (loopingSounds[sound] == null)
            {
                audioSource.loop = true;
                loopingSounds[sound] = soundGameObject;
            }
            else
            {
                Debug.LogWarning("The sound " + sound + " is already looping");
                Object.Destroy(soundGameObject);
                return;
            }
        }

        audioSource.Play();

        if(!loop)
            Object.Destroy(soundGameObject, oneShotAudioSource.clip.length);
    }
    
    public static void StopSound(Sound sound)
    {
        if (loopingSounds.ContainsKey(sound))
        {
            if (loopingSounds[sound])
            {
                Object.Destroy(loopingSounds[sound]);
                loopingSounds[sound] = null;
            }
            else
            {
                Debug.LogWarning("The sound " + sound + " is not playing");
            }
        }

    }

    public static void PlaySound(Sound sound)
    {
        if (!CanPlaySound(sound))
            return;

        if(oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("Sound");
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }

        oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default: return true;
            case Sound.Placeholder:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float playerMoveTimerMax = .05f;

                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                        return true;
                }
                else 
                    return false;
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;

        Debug.LogError("The Sound " + sound + " not found!");
        return null;

    }

 }
