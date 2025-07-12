using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamChild : MonoBehaviour
{
    private SteamObject parentSteam;

    private void Start()
    {
        parentSteam = GetComponentInParent<SteamObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (parentSteam != null && parentSteam.IsActive())
        {
            collision.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, parentSteam.GetDamage());
            
            var controller = collision.GetComponent<PlayerRespawnController>();
            if (controller != null)
            {
                controller.Respawn();
            }
        }
    }
}