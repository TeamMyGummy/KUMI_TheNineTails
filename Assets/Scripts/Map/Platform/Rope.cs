using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private List<(Transform child, Vector2 originPos)> _segments = new List<(Transform, Vector2)>();
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _segments.Add((transform.GetChild(i), transform.GetChild(i).position));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<CharacterMovement>(out var cm) && other.gameObject.TryGetComponent<PlayerController>(out var pc))
            {
                if (!cm.CheckIsGround() && !cm.CheckIsRopeClimbing() && pc.IsPressedClimbKey())
                {
                    pc.StartRopeClimb();
                    cm.StartRopeClimbState();
                    cm.Move(Vector2.up);
                    other.transform.position = new Vector2(transform.position.x, other.transform.position.y);

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        _segments[i].child.position = _segments[i].originPos;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
            {
                if (cm.CheckIsRopeClimbing())
                {
                    cm.EndRopeClimbState();
                    cm.Move(Vector2.zero);
                    other.gameObject.GetComponent<PlayerController>().EndRopeClimb();
                }
            }
        }
    }
}
