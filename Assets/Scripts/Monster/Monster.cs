// Monster.cs
using UnityEngine;
using GameAbilitySystem;

public class Monster : MonoBehaviour
{
    
    public AbilitySystem asc { get; private set; }
    
    [SerializeField] private MonsterSO monsterData;
    public MonsterSO Data => monsterData;
    
    [SerializeField] private string abilitySystemPath = "";
    private SpriteRenderer spriteRenderer;
    private float prevHp;
    public bool isAggro { get; private set; }  = false;
    
    private void Awake()
    {
        // ASC 초기화
        asc = new AbilitySystem();
        asc.SetSceneState(this.gameObject);
        asc.Init(abilitySystemPath);
        asc.GrantAllAbilities();
        prevHp = asc.Attribute.Attributes["HP"].CurrentValue.Value;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float currHp = asc.Attribute.Attributes["HP"].CurrentValue.Value;
        if (currHp < prevHp)
            StartCoroutine(Flash());
        prevHp = currHp;
        
        // 사망 처리
        if (currHp <= 0f){
            Debug.Log($"[Monster] 처치");
            Destroy(this.gameObject);
        }
            
    }

    private System.Collections.IEnumerator Flash()
    {
        var prev = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = prev;
    }

    public void SetAggroState(bool state)
    {
        isAggro = state;
    }
}