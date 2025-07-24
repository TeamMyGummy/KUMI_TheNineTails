// Monster.cs
using UnityEngine;
using GameAbilitySystem;

public class Monster : MonoBehaviour
{
    
    public AbilitySystem asc { get; private set; }
    
    [SerializeField] private MonsterSO monsterData;
    public MonsterSO Data => monsterData;
    
    [SerializeField] private string abilitySystemPath = "";
    
    private void Awake()
    {
        // ASC 초기화
        asc = new AbilitySystem();
        asc.SetSceneState(this.gameObject);
        asc.Init(abilitySystemPath);
        asc.GrantAllAbilities();
    }

    private void Update()
    {
        // 사망 처리
        if (asc.Attribute.Attributes.TryGetValue("HP", out var hp))
        {
            if (hp.CurrentValue.Value <= 0f)
            {
                Die();
                return;
            }
        }
    }

    private void Die()
    {
        Debug.Log($"[Monster] 처치");
        Destroy(this.gameObject);
    }
}