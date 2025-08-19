using System;
using GameAbilitySystem;
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
        int parryLayerMask = 1 << parryLayer;

        var size = Physics2D.OverlapBoxNonAlloc(boxCenter, boxSize, angle, _results, parryLayerMask);
        if (size > 0)
        {
            Debug.Log("패링 성공");
            //todo: 아직까지는 문제 없는데 이걸 여기다 두는 게 맞는가 -> 분리할 수 있다면 분리할 것
            AbilitySystem asc;
            DomainFactory.Instance.GetDomain(DomainKey.Player, out asc);
            asc.ApplyGameplayEffect(asc, new InstantGameplayEffect("FoxFireGauge", 1));
            if (Mathf.Approximately(asc.Attribute.Attributes["FoxFireGauge"].CurrentValue.Value, asc.Attribute.Attributes["FoxFireGauge"].MaxValue))
            {
                asc.Attribute.Attributes["FoxFireCount"].CurrentValue.Value += 1;
                asc.Attribute.Attributes["FoxFireGauge"].Reset();
            }
        }
        else
        {
            if (_attacker != null)
            {
                Transform playerTransform = GameObject.FindWithTag("Player").transform;
                Vector2 attackDirection = playerTransform.position.x > _attacker.transform.position.x ? Vector2.right : Vector2.left;
                other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, 1, attackDirection);              
            }
            else
            {
                other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, 1);
            }
        }
    }
}
