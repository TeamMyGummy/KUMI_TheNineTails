using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;
    private PlayerRespawnController _playerRespawnController;
    private Damageable _playerDamageable;
    private void Start()
    {
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
        
        _playerDamageable?.GetDamage(DomainKey.Player, damage);
        _playerRespawnController?.Respawn();
    }
}
