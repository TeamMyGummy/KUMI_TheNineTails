using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class InitDisable : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
