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

    public GameObject ropeChild;
    public int childCount; 
    
    private void Awake()
    {
        for (int i = 0; i < childCount; i++)
        {
            // 자식 생성 (부모를 this.transform으로 설정)
            GameObject child = Instantiate(ropeChild, transform);

            // 위치 조정 (원하면 랜덤 배치 가능)
            child.transform.localPosition = new Vector3(0f, 2.2f - 0.2f * i, 0f);
            
            _segments.Add((child.transform, child.transform.position));
            if (i == 0)
            {
                // 첫 번째 경우 Anchor 받아오기
                child.GetComponent<HingeJoint2D>().connectedBody = transform.GetChild(0).GetComponent<Rigidbody2D>();
            }
            else
            {
                child.GetComponent<HingeJoint2D>().connectedBody = _segments[i - 1].child.GetComponent<Rigidbody2D>();
                child.GetComponent<HingeJoint2D>().anchor = new Vector2(0, 0.074f);
                child.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -0.138f);
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //_segments.Add((transform.GetChild(i), transform.GetChild(i).position));
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

                /*for (int i = 0; i < transform.childCount; i++)
                {
                    _segments[i].child.position = _segments[i].originPos;
                }*/
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
