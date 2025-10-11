using GameAbilitySystem;
using UnityEngine;

public class SteamChild : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 1.0f;
    
    private SteamObject _parentSteam;

    private void Awake()
    {
        _parentSteam = GetComponentInParent<SteamObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (_parentSteam == null || !_parentSteam.IsActive()) return;

        DomainFactory.Instance.GetDomain(DomainKey.Player, out AbilitySystem asc);
        if (asc == null) return;

        // 이미 무적이면 무시
        if (asc.TagContainer.Has(GameplayTags.Invincibility)) return;

        var dmg  = collision.GetComponent<Damageable>();
        var resp = collision.GetComponent<PlayerRespawnController>();
        if (dmg == null || resp == null) return;

        // 1회 데미지
        dmg.GetDamage(DomainKey.Player, _parentSteam.GetDamage());

        // 리스폰 요청
        resp.StartRespawn(respawnDelay);
    }
}