using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour
{
    [SerializeField] private ConditionKey conditionKey;
    
    
    public void OpenDoor(){
		ConditionEventBus.Raise(conditionKey);
	}
}
