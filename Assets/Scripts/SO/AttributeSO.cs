using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttributeSO")]
public class AttributeSO : ScriptableObject
{
    public string AttributeName;
    public float BaseValue;
    public float MaxValue;
}
