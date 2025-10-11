using GameAbilitySystem;
using UnityEngine;

public class SpikeLaserObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;      
    [SerializeField] private float respawnDelay = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        DomainFactory.Instance.GetDomain(DomainKey.Player, out AbilitySystem asc);
        if (asc == null) return;

        // 이미 무적이면(다른 해저드에 먼저 맞았거나 리스폰 중) 무시
        if (asc.TagContainer.Has(GameplayTags.Invincibility)) return;

        var dmg  = collision.GetComponent<Damageable>();
        var resp = collision.GetComponent<PlayerRespawnController>();
        if (dmg == null || resp == null) return;

        // 1회 데미지
        dmg.GetDamage(DomainKey.Player, damage);

        // 리스폰 요청
        resp.StartRespawn(respawnDelay);
    }
}