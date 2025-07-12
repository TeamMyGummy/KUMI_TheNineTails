using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamChild : MonoBehaviour
{
    private SteamObject parentSteam;
    private PlayerRespawnController _playerRespawnController;
    private Damageable _playerDamageable;

    private void Start()
    {
        parentSteam = GetComponentInParent<SteamObject>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _playerRespawnController = player.GetComponent<PlayerRespawnController>();
            _playerDamageable = player.GetComponent<Damageable>();
        }

        if (_playerRespawnController == null || _playerDamageable == null)
        {
            Debug.LogError("[SpikeObject] 플레이어 관련 컴포넌트를 찾을 수 없습니다");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        if (parentSteam != null && parentSteam.IsActive())
        {
            _playerDamageable?.GetDamage(DomainKey.Player, parentSteam.GetDamage());
            _playerRespawnController?.Respawn();
        }
    }
}
