using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrigger : MonoBehaviour
{
    private Rock1 _rock;

    void Awake()
    {
        _rock = transform.parent.GetComponentInChildren<Rock1>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _rock.TriggerFall();
        }
    }
}
