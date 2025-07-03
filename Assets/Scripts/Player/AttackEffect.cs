using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemies")
        {
            collision.gameObject.GetComponent<Damageable>().GetDamage(2.0f);
        }
    }
}
