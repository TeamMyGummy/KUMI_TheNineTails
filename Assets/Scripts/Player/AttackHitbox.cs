    using GameAbilitySystem;
    using UnityEngine;

    public class AttackHitbox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            AbilitySystem asc = collision.GetComponent<IAbilitySystem>().asc;
            collision.gameObject.GetComponent<Damageable>().GetDamage(asc, 10.0f);
        }
    }
