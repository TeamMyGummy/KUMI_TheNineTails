using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeLaserObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float respawnDelay = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Damageable>().GetDamage(DomainKey.Player, damage);
            
            var controller = collision.GetComponent<PlayerRespawnController>();
            if (controller != null)
            {
                StartCoroutine(RespawnAfterDelay(controller));
            }
        }
    }
    
    // 장애물에 닿으면 딜레이 후 리스폰
    private IEnumerator RespawnAfterDelay(PlayerRespawnController controller)
    {
        yield return new WaitForSeconds(respawnDelay);
        controller.Respawn();
    }
}
