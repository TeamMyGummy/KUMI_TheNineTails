// Monster.cs
using UnityEngine;
using GameAbilitySystem;

public class Monster : MonoBehaviour
{
    
    public AbilitySystem asc { get; private set; }

    [SerializeField] private string monsterName = "";
    [SerializeField] private bool isCreature = true; //생물(true) / 기계(false)
    [SerializeField] private bool isMoving = true; //능동(true) /수동(false)

    private void Awake()
    {
        // ASC 초기화
        asc = new AbilitySystem();
        asc.SetActor(this.gameObject);
        asc.Init("Test/TestMonster1");
        asc.GrantAllAbilities();
    }

    private float damageTimer = 0f;

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

        //여기서부턴 hp바 잘 뜨는지 보기 위한 임시 데미지입히기코드..
        damageTimer += Time.deltaTime;
        if (damageTimer >= 2f)
        {
            damageTimer = 0f;

            if (asc.Attribute.Attributes.TryGetValue("HP", out var HP))
            {
                hp.Modify(-10f, ModOperation.Additive);
                Debug.Log($"[Test] {monsterName}에게 10 데미지를 입혔습니다. 남은 체력: {hp.CurrentValue.Value}");
            }
        }
    }

    private void Die()
    {
        Debug.Log($"[Monster] {monsterName} 처치");
        Destroy(this.gameObject);
    }
}