using UnityEngine;
using UnityEngine.UI;

public class BtnVisuals : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] Sprite normal, pressed;

    public void OnPress()
    {
        btn.image.sprite = pressed;
        SoundManager.PlaySound(SoundManager.Sound.BtnPress);
    }

    public void OnRelease()
    {
        btn.image.sprite = normal;
    }

}
