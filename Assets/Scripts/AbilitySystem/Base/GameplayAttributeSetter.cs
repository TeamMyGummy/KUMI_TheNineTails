using System.Collections.Generic;
using AbilitySystem.Base;
using Data;
using UnityEngine;


[DefaultExecutionOrder(-99)]
public class GameplayAttributeSetter : MonoBehaviour
{
    public List<AttributeSO> AddAttributeSO;
    private AbilitySystem.Base.AbilitySystem _playerModel;

    public void Awake()
    {
        if (DomainFactory.Instance.TryGetDomain(SaveKey.Player, _playerModel)) return;
        _playerModel = new();
        foreach (var att in AddAttributeSO)
        {
            _playerModel.Attribute.CreateAttribute(att);
        }
        DomainFactory.Instance.RegisterDomain<ASCState>(SaveKey.Player, _playerModel);
    }
}
