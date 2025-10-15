using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour
{
    [SerializeField] private ConditionKey conditionKey;
    
    [SerializeField] private Image progressBar;
    [SerializeField] private float holdDuration = 5f; // 총 눌러야 하는 시간
    
    private float _currentHoldTime = 0f;
    private bool _isHolding = false;

    private void Update()
    {
        if (_isHolding)
        {
            return;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            _currentHoldTime += Time.deltaTime;
            progressBar.fillAmount = _currentHoldTime / holdDuration;
            if (_currentHoldTime >= holdDuration)
            {
                ConditionEventBus.Raise(conditionKey);
                _isHolding = true;
            }
        }
        
    }
}
