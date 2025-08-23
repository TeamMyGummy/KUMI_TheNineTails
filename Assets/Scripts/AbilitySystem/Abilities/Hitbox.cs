using System;
using GameAbilitySystem;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string parryLayerName = "Parrying";

    private BoxCollider2D _boxCollider;
    private readonly Collider2D[] _results = new Collider2D[1];

    private GameObject _attacker;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetAttacker(GameObject attacker)
    {
        _attacker = attacker;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_boxCollider == null) return;
        
        Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset;
        Vector2 boxSize = _boxCollider.size;
        float angle = 0f;

        int parryLayer = LayerMask.NameToLayer(parryLayerName);
        if (parryLayer != -1)
        {
            int parryLayerMask = 1 << parryLayer;

            var size = Physics2D.OverlapBoxNonAlloc(boxCenter, boxSize, angle, _results, parryLayerMask);
            if (size > 0)
            {
                // 플레이어 패링 성공
                Monster monster = _attacker?.GetComponent<Monster>();
                if (monster != null && 
                    monster.asc.Attribute.Attributes["HP"].CurrentValue.Value <=
                    monster.asc.Attribute.Attributes["HP"].MaxValueRP.Value * 0.4f)
                {
                    // 간 빼기 스킬 활성화
                    other.GetComponent<ParryingHitbox>()?.StartLiverExtraction();
                }
                else
                {
                    other.GetComponent<ParryingHitbox>()?.Parrying();
                }
            }
        }
        else
        {
            // 플레이어 피격&넉백
            Transform playerTransform = GameObject.FindWithTag("Player").transform;
            if (_attacker != null)
            {
                // 몬스터가 있을 때 : 몬스터 위치 기준
                Vector2 attackDirection = playerTransform.position.x > _attacker.transform.position.x ? Vector2.right : Vector2.left;
                other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, 1, attackDirection);              
            }
            else
            {
                // 몬스터가 없을 때 (+ 투사체일 때) : HitBox 위치 기준
                Vector2 attackDirection = playerTransform.position.x > transform.position.x ? Vector2.right : Vector2.left;
                other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, 1, attackDirection);
            }
        }
    }
}
