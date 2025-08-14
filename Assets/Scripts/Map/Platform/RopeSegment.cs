using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    private Rigidbody2D _rigid;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
            {
                Vector2 forceDir = cm.GetCharacterSpriteDirection();
                _rigid.AddForce(forceDir * 0.01f, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
            {
                Vector2 forceDir = cm.GetCharacterSpriteDirection();
                _rigid.AddForce(forceDir * 0.01f, ForceMode2D.Impulse);
            }
        }
    }
}
