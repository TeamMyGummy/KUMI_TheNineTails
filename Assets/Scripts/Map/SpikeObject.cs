using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;

public class SpikeObject : MonoBehaviour
{
    [SerializeField] private float damage = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Damageable>().GetDamage(DomainKey.Player, damage);
            
            var controller = collision.GetComponent<PlayerRespawnController>();
            if (controller != null)
            {
                controller.Respawn();
            }
        }
    }
}