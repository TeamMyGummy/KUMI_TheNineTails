using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string parryLayerName = "Parrying";

    private BoxCollider2D _boxCollider;
    private readonly Collider2D[] _results = new Collider2D[1];

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_boxCollider == null) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster")) return;
        
        Debug.Log(other.gameObject.name);

        Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset;
        Vector2 boxSize = _boxCollider.size;
        float angle = 0f;

        int parryLayer = LayerMask.NameToLayer(parryLayerName);
        int parryLayerMask = 1 << parryLayer;

        var size = Physics2D.OverlapBoxNonAlloc(boxCenter, boxSize, angle, _results, parryLayerMask);
        if (size > 0)
        {
            Debug.Log("패링 성공"); //todo. 패링 성공이 두 번 뜨는 문제(플레이어까지 닿으면... )
        }
        else
        {
            other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, 1);
        }
    }
}
