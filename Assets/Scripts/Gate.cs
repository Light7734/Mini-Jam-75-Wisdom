using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int crystalsNeeded = 2;
    public int crystalsAquired = 0;
    public bool open = false;

    private List<GameObject> crystals = new List<GameObject>();
    public List<GameObject> sockets = new List<GameObject>();

    public void AddCrystal(GameObject crystal)
    {
        SoundManager.i.PlaySound(SoundManager.Sound.CrystalInsert);

        crystalsAquired++;
        sockets[crystalsAquired - 1].SetActive(true);
        crystals.Add(crystal);

        if (crystalsAquired == crystalsNeeded)
        {
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


            sockets[crystalsAquired - 1].SetActive(false);
            crystalsAquired--;

            return crystal;
        }

        return null;
    }

}
