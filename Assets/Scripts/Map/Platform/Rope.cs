using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private List<(Transform child, Vector2 originPos)> _segments = new List<(Transform, Vector2)>();
    
    private CharacterMovement _cm;
    private PlayerController _pc;

    private bool _prePressedKey;
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _segments.Add((transform.GetChild(i), transform.GetChild(i).position));
        }

        _prePressedKey = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _cm = other.gameObject.GetComponent<CharacterMovement>();
            _pc = other.gameObject.GetComponent<PlayerController>();

            _prePressedKey = _pc.IsPressedClimbKey(); // 미리 누르고 있었는지
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _cm != null && _pc != null)
        {
            if (!_cm.CheckIsGround() && !_cm.CheckIsRopeClimbing() && _pc.IsPressedClimbKey() && !_prePressedKey)
            {
                _cm.StartRopeClimbState();
                _pc.StartRopeClimb();
                _cm.Move(Vector2.up);
                other.transform.position = new Vector2(transform.position.x, other.transform.position.y);

                for (int i = 0; i < transform.childCount; i++)
                {
                    _segments[i].child.position = _segments[i].originPos;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _cm != null && _pc != null)
        {
            // 점프 안 하고 그냥 내려왔을 때
            if (_cm.CheckIsRopeClimbing())
            {
                _cm.EndRopeClimbState();
                _pc.EndRopeClimb();
                _cm.Move(Vector2.zero);
            }
        }
    }
}
