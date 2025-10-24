using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartImugi : MonoBehaviour
{
    [SerializeField] private GameObject imugi;
    
    
    public void ActivateImugi()
    {
        imugi.SetActive(true);
    }
}
