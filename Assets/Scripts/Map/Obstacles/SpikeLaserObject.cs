using GameAbilitySystem;
using UnityEngine;

public class SpikeLaserObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float respawnDelay = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision) => TryHit(collision);
    private void OnTriggerStay2D(Collider2D collision)  => TryHit(collision);

    private void TryHit(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var resp = collision.GetComponent<PlayerRespawnController>();
        if (resp == null) return;

        // 어느 쪽이든 무적이면 통과
        var player = collision.GetComponent<Player>();
        bool tagInv = player != null && player.ASC.TagContainer.Has(GameplayTags.Invincibility);
        if (resp.IsRespawningOrInvincible || tagInv) return;

        // 데미지 & 리스폰
        var dmg = collision.GetComponent<Damageable>();
        if (dmg == null) return;

        dmg.GetDamage(DomainKey.Player, damage);
        resp.StartRespawn(respawnDelay);
    }
}