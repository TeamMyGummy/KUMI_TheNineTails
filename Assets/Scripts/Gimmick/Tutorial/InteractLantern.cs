using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractLantern : MonoBehaviour
{
    [SerializeField] private ConditionKey condition;
    [SerializeField] private LanternObject lantern;

    private void Awake()
    {
        if (lantern == null) return;
        lantern.Interacted += OnLanternActivated;
    }

    private void OnLanternActivated(int lanternKey)
    {
        ConditionEventBus.Raise(condition);
        lantern.Interacted -= OnLanternActivated;
    }
}
