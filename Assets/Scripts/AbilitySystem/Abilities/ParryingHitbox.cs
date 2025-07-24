using Unity.VisualScripting;
using UnityEngine;

public class ParryingHitbox : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            other.enabled = false;
        }
    }
}
