using System;
using System.Collections.Generic;
using AbilitySystem.Base;
using Data;
using UnityEngine;


[DefaultExecutionOrder(-98)]
public class GameplayAttributeSetter : MonoBehaviour
{
    public List<AttributeSO> AddAttributeSO;
    private AbilitySystem.Base.AbilitySystem _playerModel;

    public void Awake()
    {
        if (DomainFactory.Instance.TryGetDomain(SaveKey.Player, out _playerModel)) return;
        _playerModel = new();
        foreach (var att in AddAttributeSO)
        {
            _playerModel.Attribute.CreateAttribute(att);
        }
        DomainFactory.Instance.RegisterDomain(SaveKey.Player, _playerModel);
    }
}
