using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractLantern : MonoBehaviour
{
    [SerializeField] private ConditionKey condition;
    [SerializeField] private LanternObject lantern;

    private bool _isActive = false;
    private void Update()
    {
        
        if (lantern.isInteracting && !_isActive)
        {
            Debug.Log("호롱불");
            ConditionEventBus.Raise(condition);
            _isActive = true;
        }
    }
}
