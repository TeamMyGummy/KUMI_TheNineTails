using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

//ASC가 붙어있지 않은 오브젝트로 Damagable 판정이 필요할 경우
public class AbilityHandler : MonoBehaviour, IAbilitySystem
{
    [SerializeField] 
    [Tooltip("IAbilitySystem이 부착된 오브젝트를 드래그 드롭하시오.")] 
    private GameObject gameObjectWithASC;
    private IAbilitySystem _abilitySystem;
    public AbilitySystem asc => _abilitySystem.asc;

    void Awake()
    {
        gameObjectWithASC.GetComponent<IAbilitySystem>();
    }
}
