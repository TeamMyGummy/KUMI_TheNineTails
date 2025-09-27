using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private List<(Transform child, Vector2 originPos)> _segments = new List<(Transform, Vector2)>();

    private Player _player;
    private CharacterMovement _cm;
    private PlayerController _pc;
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _segments.Add((transform.GetChild(i), transform.GetChild(i).position));
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _player = other.gameObject.GetComponent<Player>();
            _cm = _player.Movement;
            _pc = _player.Controller;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_cm.CheckIsGround() && !_player.StateMachine.IsCurrentState(PlayerStateType.RopeClimb) && _pc.ClimbInput != Vector2.zero)
            {
                _player.OnRopeClimbAvailable?.Invoke(true);
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
            _player.OnRopeClimbAvailable?.Invoke(false);
        }
    }
}
