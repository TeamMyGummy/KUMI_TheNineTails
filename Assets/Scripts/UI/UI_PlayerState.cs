using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

public class UI_PlayerState : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hp;
    private PlayerState _playerModel;

    void Awake()
    {
        _playerModel = DataManager.Instance.Player;
    }
    
    void Start()
    {
        UpdateUp(_playerModel.Attributes["HP"].CurrentValue);
        _playerModel.Attributes["HP"].OnValueChanged -= UpdateUp;
        _playerModel.Attributes["HP"].OnValueChanged += UpdateUp;
    }

    //추후 해당 코드 변경
    public void UpdateUp(float hp)
    {
        _hp.text = hp.ToString();
    }
}
