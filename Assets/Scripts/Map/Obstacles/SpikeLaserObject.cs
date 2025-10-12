using UnityEngine;

public class SpikeLaserObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float respawnDelay = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 1) 리스폰/무적 플래그만 본다 (단일 소스)
        var resp = collision.GetComponent<PlayerRespawnController>();
        if (resp == null) return;
        if (resp.IsRespawningOrInvincible) return;

        // 2) 데미지 & 리스폰
        var dmg = collision.GetComponent<Damageable>();
        if (dmg == null) return;

        dmg.GetDamage(DomainKey.Player, damage);
        resp.StartRespawn(respawnDelay);
    }
}