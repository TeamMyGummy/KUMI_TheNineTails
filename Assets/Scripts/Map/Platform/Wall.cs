using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Bounds _wallBounds;

    private CharacterMovement _cm;
    private PlayerController _pc;

    private void Start()
    {
        _wallBounds = GetComponent<Collider2D>().bounds;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           _cm = other.gameObject.GetComponent<CharacterMovement>();
           _pc = other.gameObject.GetComponent<PlayerController>(); 
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && _cm != null && _pc != null)
        {
            if (!_cm.CheckIsWallClimbing())
            {
                if (!_cm.CheckIsGround())
                {
                     // 벽타기 활성화
                     float characterPos = other.transform.position.x < transform.position.x ? -1.0f : 1.0f; // 캐릭터가 왼쪽에 있는지 오른쪽에 있는지
                     float characterSpriteDir = _cm.GetCharacterSpriteDirection().x;
                     
                     if (Mathf.Abs(characterPos - characterSpriteDir) > 0.01f)
                     {
                         // 벽에 대한 위치와 바라보는 방향이 반대여야 함
                         _cm.StartWallClimbState();  
                         _pc.StartWallClimb(gameObject);                 
                     }
                }
            }
            // Player가 꼭대기에 도달했는지 확인
            else if (_cm.CheckIsWallClimbing())
            {
                if (other.bounds.center.y > _wallBounds.max.y)
                {
                    other.gameObject.GetComponent<Player>().LedgeClimb(true);
                    if(_cm.GetCharacterDirection() == Vector2.up)
                        _cm.Jump(2.0f);
                }
                else
                {
                    other.gameObject.GetComponent<Player>().LedgeClimb(false);
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && _cm != null && _pc != null)
        {
            // 벽타기 비활성화
            _cm.EndWallClimbState();
            _pc.EndWallClimb();
        }
    }

}
