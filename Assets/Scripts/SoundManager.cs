using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    static private SoundManager _i;

    public static SoundManager i
    {
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("SoundManager")) as GameObject).GetComponent<SoundManager>();
                _i.Initialize();
                
                DontDestroyOnLoad(_i);
            }

            return _i;
        }
    }

    public enum Sound 
    {
        StartMenuBackground,
        GameBackground,

        BtnPress,
        BtnRelease,

        GateOpen,
        PressurePlate,

        CrystalInsert,
        CrystalRemove,
        CrystalPickupRed,
        CrystalPickupBlue,
        CrystalDrop,

        Death,

        highJump,
        normalJump,
        lowJump,

        Placeholder,
    }

    private  Dictionary<Sound, float> soundTimeDictionary;

    private  Dictionary<Sound, GameObject> loopingSounds;
    private  Dictionary<Sound, bool> stopFadeIn;

    private  GameObject oneShotGameObject;
    private  AudioSource oneShotAudioSource;

    public AudioMixer mixer;

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("musicVolume", volume);
    }

    public void Initialize()
    {
        soundTimeDictionary = new Dictionary<Sound, float>();
        soundTimeDictionary[Sound.Placeholder] = 0;

        stopFadeIn = new Dictionary<Sound, bool>();
        stopFadeIn[Sound.StartMenuBackground] = false;
        stopFadeIn[Sound.GameBackground] = false;

        loopingSounds = new Dictionary<Sound, GameObject>();
        loopingSounds[Sound.StartMenuBackground] = null;
        loopingSounds[Sound.GameBackground] = null;
    }

    public void PlaySound(Sound sound, Vector3 position, bool loop = false, bool fadeIn = false)
    {
        if (!CanPlaySound(sound))
            return;

        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.position = position;
        DontDestroyOnLoad(soundGameObject);

        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        audioSource.clip = GetAudioClip(sound);

        if (loop && loopingSounds.ContainsKey(sound))
        {
            if (loopingSounds[sound] == null)
            {
                audioSource.loop = true;
                loopingSounds[sound] = soundGameObject;
            }
            else
            {
                Debug.LogWarning("The sound " + sound + " is already looping");
                Destroy(soundGameObject);
                return;
            }
        }

        if(fadeIn)
        {
            audioSource.volume = 0f;
            stopFadeIn[sound] = false;
            i.StartCoroutine(FadeIn(audioSource, sound));
        }

        audioSource.Play();

        if(!loop)
            Destroy(soundGameObject, oneShotAudioSource.clip.length);
    }

    private IEnumerator FadeIn(AudioSource source, Sound sound)
    {
        float diffTo1 = 1.0f - source.volume;
        while (source.volume < 1f)
        {
            source.volume += diffTo1 / (1.0f / 0.1f); ;

            if (!stopFadeIn[sound])
                yield return new WaitForSeconds(0.1f);
        }

        if(source.volume >= 1f)
            source.volume = 1f;
    }

    private IEnumerator FadeOut(GameObject gameObject, AudioSource source, Sound sound)
    {
        float diffTo0 = source.volume;
        while (source.volume > 0f)
        {
            stopFadeIn[sound] = true;
            source.volume -= diffTo0 / (1.0f / 0.1f);
            yield return new WaitForSeconds(0.1f);
        }

        if (source.volume <= 0f)
            Destroy(gameObject);
    }

    public void StopSound(Sound sound, bool fadeOut = false)
    {
        if (loopingSounds.ContainsKey(sound))
        {
            if (loopingSounds[sound] != null)
            {
                if (fadeOut)
                {
                    i.StartCoroutine(FadeOut(loopingSounds[sound], loopingSounds[sound].GetComponent<AudioSource>(), sound));
                    loopingSounds[sound] = null;
                    return;
                }

                Destroy(loopingSounds[sound]);
                loopingSounds[sound] = null;
            }
            else
                Debug.LogWarning("The sound " + sound + " is not playing");
        }
    }

    public void PlaySound(Sound sound)
    {
        if (!CanPlaySound(sound))
            return;

        if(oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("Sound");
            DontDestroyOnLoad(oneShotGameObject);
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }

        oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    private bool CanPlaySound(Sound sound)
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

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;

        return null;
    }

 }
