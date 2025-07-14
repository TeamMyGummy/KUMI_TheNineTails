// Monster.cs
using UnityEngine;
using GameAbilitySystem;

public class Monster : MonoBehaviour
{
    
    public AbilitySystem asc { get; private set; }

    [SerializeField] private string monsterName = "";
    [SerializeField] private bool isParrying = true; //패링가능하면 true -> 필요할지 모르겠는데 일단 넣어놓음
    [SerializeField] private bool isMoving = true; //능동(true) /수동(false)

    private void Awake()
    {
        // ASC 초기화
        asc = new AbilitySystem();
        asc.SetActor(this.gameObject);
        asc.Init("Test/TestMonster1");
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
        Debug.Log($"[Monster] {monsterName} 처치");
        Destroy(this.gameObject);
    }
}