using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public bool CanWallClimb { get; private set; }

    private void Start()
    {
        CanWallClimb = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CanWallClimb)
        {
            if(other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
            {
                if (!cm.CheckIsGround())
                {
                     // 벽타기 활성화
                     if (other.gameObject.CompareTag("Player"))
                     {
                         CanWallClimb = true;
                         other.gameObject.GetComponent<PlayerController>().StartWallClimb(gameObject);
                     }
                     cm.StartWallClimbState();               
                }
            }    
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
        {
            // 벽타기 비활성화
            if (other.gameObject.CompareTag("Player"))
            {
                CanWallClimb = false;
                other.gameObject.GetComponent<PlayerController>().EndWallClimb();
            }
            cm.EndWallClimbState();
        }
    }

}
