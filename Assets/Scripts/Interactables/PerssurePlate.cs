using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerssurePlate : MonoBehaviour
{
    public Animator doorAnimator;
    public Animator plateAnimator;

    public int neededWeight = 1;

    public void ApplyPressure(int weight)
    {
        Debug.Log(weight);
        if(weight <= neededWeight)
        {
            SoundManager.i.PlaySound(SoundManager.Sound.PressurePlate);

            doorAnimator.SetTrigger("open");
            plateAnimator.SetTrigger("open");
        }
    }

}
