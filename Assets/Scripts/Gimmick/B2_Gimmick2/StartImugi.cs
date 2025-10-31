using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartImugi : MonoBehaviour
{
    [SerializeField] private GameObject imugi;
    [SerializeField] private MeshRenderer imugiRenderer;
    
    
    public void ActivateImugi()
    {
        imugi.SetActive(true);
        StartCoroutine(VisibleImugi());
    }

    private IEnumerator VisibleImugi()
    {
        yield return new WaitForSeconds(0.5f);
        imugiRenderer.enabled = true;
    }
}
