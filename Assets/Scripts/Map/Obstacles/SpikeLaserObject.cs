using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeLaserObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float respawnDelay = 1.0f;
    
    private bool _isProcessing = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isProcessing) return;
        
        if (collision.CompareTag("Player"))
        {
            //데미지 입히기
            collision.gameObject.GetComponent<Damageable>().GetDamage(DomainKey.Player, damage);
            
            //리스폰
            var controller = collision.GetComponent<PlayerRespawnController>();
            var pc = collision.GetComponent<PlayerController>();
            if (controller != null)
            {
                _isProcessing = true;
                StartCoroutine(RespawnAfterDelay(controller, pc));
            }
        }
    }
    
    // 장애물에 닿으면 딜레이 후 리스폰
    private IEnumerator RespawnAfterDelay(PlayerRespawnController controller, PlayerController pc)
    {
        pc.OnDisableAllInput(); //플레이어 이동 막기
        yield return new WaitForSeconds(respawnDelay);
        controller.Respawn();
        pc.OnEnableAllInput();
    }
}
