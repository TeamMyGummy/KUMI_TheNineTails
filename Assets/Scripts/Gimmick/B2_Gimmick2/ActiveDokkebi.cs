using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDokkebi : MonoBehaviour
{
    [SerializeField] private GameObject dokkebi;

    public void ActivateDokkebi()
    {
        dokkebi.SetActive(true);
    }
}
