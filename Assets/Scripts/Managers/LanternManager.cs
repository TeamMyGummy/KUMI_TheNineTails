using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static LanternManager Instance { get; private set; }
    private Lantern activeLantern;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetActiveLantern(Lantern newLantern)
    {
        if (activeLantern != null && activeLantern != newLantern)
            activeLantern.SetState(LanternState.SmallFlame);

        activeLantern = newLantern;
        activeLantern.SetState(LanternState.BigFlame);
    }

    
}
