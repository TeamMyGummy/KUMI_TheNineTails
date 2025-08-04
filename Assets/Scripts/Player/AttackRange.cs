using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private GameObject _actor;
    private readonly Vector2 _spawnPoint = new Vector2(0.5f, 0.75f);

    void Start()
    {
        _actor = GetComponentInParent<Player>().gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if(collision.gameObject.CompareTag("Enemies"))
        {*/
            AbilitySystem asc = collision.GetComponent<Monster>().asc;
            collision.gameObject.GetComponent<Damageable>().GetDamage(asc, 10.0f);
        //}

        if (collision.gameObject.CompareTag("BreakableWall"))
        {
            collision.gameObject.GetComponent<BreakableWall>().AttackCount();
        }
    }

    public void SpawnAttackRange()
    {
        gameObject.transform.localPosition = _actor.GetComponent<SpriteRenderer>().flipX 
            ? new Vector2(_spawnPoint.x * (-1), _spawnPoint.y) 
            : new Vector2(_spawnPoint.x, _spawnPoint.y);
    }
    public void EnableAttackCollider(bool enabled)
    {
        GetComponent<Collider2D>().enabled = enabled;
    }
}
