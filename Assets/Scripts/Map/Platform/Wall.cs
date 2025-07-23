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

    private void OnTriggerEnter2D(Collider2D other)
    {
        CanWallClimb = true;
        if(other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
        {
            if (!cm.CheckIsGround())
            {
                 // 벽타기 활성화
                 if (other.gameObject.CompareTag("Player"))
                 {
                     other.gameObject.GetComponent<PlayerController>().OnEnableWallClimb();
                     other.gameObject.GetComponent<Player>().SetWallClimb(true);
                 }
                 cm.StartWallClimb();               
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CanWallClimb = false;
        if(other.gameObject.TryGetComponent<CharacterMovement>(out var cm))
        {
            // 벽타기 활성화
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().OnDisableWallClimb();
                other.gameObject.GetComponent<Player>().SetWallClimb(false);
            }
            cm.EndWallClimb();
        }
    }

}
