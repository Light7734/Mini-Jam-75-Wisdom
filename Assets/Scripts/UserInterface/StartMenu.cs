using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameObject mainMenuGameObject, settingsGameObject;

    public void Start()
    {
        SoundManager.i.PlaySound(SoundManager.Sound.StartMenuBackground, Vector3.zero, true, true);
    }

    public void PlayBtnPressed()
    {
        SoundManager.i.PlaySound(SoundManager.Sound.BtnRelease);
        SoundManager.i.StopSound(SoundManager.Sound.StartMenuBackground, true);
        sceneLoader.LoadScene(1);
    }

    public void PrefBtnPressed()
    {
        SoundManager.i.PlaySound(SoundManager.Sound.BtnRelease);
        mainMenuGameObject.SetActive(false);
        settingsGameObject.SetActive(true);
    }

    public void QuitBtnPressed()
    {
        SoundManager.i.PlaySound(SoundManager.Sound.BtnRelease);
        Application.Quit();
    }

    public void BackBtnPressed()
    {
        SoundManager.i.PlaySound(SoundManager.Sound.BtnRelease);
        mainMenuGameObject.SetActive(true);
        settingsGameObject.SetActive(false);
    }

}