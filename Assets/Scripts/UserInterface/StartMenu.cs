using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour
{
    [SerializeField] Button playBtn, prefBtn, quitBtn;
    [SerializeField] Sprite playBtnNormal, playBtnPress, prefBtnNormal, prefBtnPress, quitBtnNormal, quitBtnPress;

    public void Start()
    {
        SoundManager.Initialize();
        SoundManager.PlaySound(SoundManager.Sound.StartMenuBackground, Vector3.zero, true);
    }

    public void PlayBtnPressed()
    {
        SoundManager.PlaySound(SoundManager.Sound.BtnClick);
        SceneManager.LoadScene(1);
    }

    public void PrefBtnPressed()
    {
        SoundManager.PlaySound(SoundManager.Sound.BtnClick);
        Debug.Log("!!");
    }

    public void QuitBtnPressed()
    {
        SoundManager.PlaySound(SoundManager.Sound.BtnClick);
        Application.Quit();
    }


    #region VISUALS

    public void PlayBtnDown()
    {
        playBtn.image.sprite = playBtnPress;
    }

    public void PlayBtnReleased()
    {
        playBtn.image.sprite = playBtnNormal;
    }

    public void PrefBtnDown()
    {
        prefBtn.image.sprite = prefBtnPress;
    }

    public void PrefBtnReleased()
    {
        prefBtn.image.sprite = prefBtnNormal;
    }

    public void QuitBtnDown()
    {
        quitBtn.image.sprite = quitBtnPress;
    }

    public void QuitBtnReleased()
    {
        quitBtn.image.sprite = quitBtnNormal;
    }

    #endregion // __VISUALS__ //
}