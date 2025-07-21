using System;
using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using Unity.VisualScripting;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private GameObject _actor;
    private CharacterMovement _cm;
    private Vector2 _spawnPoint = new Vector2(0.4f, 0.75f);

    void Start()
    {

    }

    private void OnEnable()
    {
        /*_actor = GetComponentInParent<Player>().gameObject;
        _cm = _actor.GetComponent<CharacterMovement>();
        gameObject.transform.position = new Vector2(_cm.GetCharacterDirection().x * _spawnPoint.x, _spawnPoint.y);*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemies")
        {
            AbilitySystem asc = collision.GetComponent<Monster>().asc;
            collision.gameObject.GetComponent<Damageable>().GetDamage(asc, 2.0f);
        }
    }
}
