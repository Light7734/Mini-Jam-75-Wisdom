using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int crystalsNeeded = 2;
    public int crystalsAquired = 0;
    private int blueCrystals = 0;
    private int redCrystals = 0;
    public bool open = false;

    private List<GameObject> crystals = new List<GameObject>();
    public List<GameObject> blueSockets = new List<GameObject>();
    public List<GameObject> redSockets = new List<GameObject>();


    public void AddCrystal(GameObject crystal)
    {
        SoundManager.i.PlaySound(SoundManager.Sound.CrystalInsert);

        crystalsAquired++;
        Debug.Log(crystalsAquired - 1);

        if(crystal.tag == "blueCrystal")
        {
            blueCrystals++;
            blueSockets[blueCrystals - 1].SetActive(true);
        }
        else if (crystal.tag == "redCrystal")
        {
            redCrystals++;
            redSockets[redCrystals - 1].SetActive(true);
        }

        crystals.Add(crystal);

        if (crystalsAquired == crystalsNeeded)
        {
            SoundManager.i.PlaySound(SoundManager.Sound.GateOpen);
            open = true;
            GetComponent<Animator>().SetTrigger("Open");
        }
    }

    public GameObject RemoveCrystal()
    {
        if (open)
            return null;

        if (crystals.Count != 0)
        {
            SoundManager.i.PlaySound(SoundManager.Sound.CrystalRemove);

            GameObject crystal = crystals[crystals.Count - 1];
            crystals.RemoveAt(crystals.Count - 1);

            if (crystal.tag == "blueCrystal")
            {
                blueSockets[blueCrystals - 1].SetActive(false);
                blueCrystals--;
            }
            else if (crystal.tag == "redCrystal")
            {
                redSockets[redCrystals - 1].SetActive(false);
                redCrystals--;
            }
            crystalsAquired--;

            return crystal;
        }

        return null;
    }

}
