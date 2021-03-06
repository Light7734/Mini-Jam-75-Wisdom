using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Image image;

    public void ChangeMasterVolume(float volume)
    {
        SoundManager.i.SetMasterVolume(volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        SoundManager.i.SetMusicVolume(volume);
    }

}
