using GameAbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviorTree
{
    public class BTContext : MonoBehaviour, IAbilitySystem
    {
        [SerializeField] private AbilitySystemSO so;
        public AbilitySystem asc => ASC;
        public AbilitySystem ASC = new();
        public bool bIdleState = false;

        void Awake()
        {
            ASC?.Init(so);
            ASC?.GrantAllAbilities();
            ASC?.SetSceneState(gameObject);
        }

    }
}