using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisableAfterHitted : MonoBehaviour
{
    private Collider2D _collider2D;

    public void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _collider2D.enabled = false;
        }
    }
}
