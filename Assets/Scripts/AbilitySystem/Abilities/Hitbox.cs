using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        collision.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, 1);
    }
}
