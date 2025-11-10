using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckClimbing : MonoBehaviour
{
    [SerializeField] private ConditionKey condition;

    public void OpenDoor()
    {
        ConditionEventBus.Raise(condition);
    }
}
