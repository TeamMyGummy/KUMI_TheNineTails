using System.Collections.Generic;
using AbilitySystem.Base;
using UnityEngine;


[DefaultExecutionOrder(-99)]
public class GameplayAttributeSetter : MonoBehaviour
{
    public List<AttributeSO> AddAttributeSO;

    public void Awake()
    {
        foreach (var att in AddAttributeSO)
        {
            DomainFactory.Instance.PlayerASC.Attribute.CreateAttribute(att);
        }
    }
}
