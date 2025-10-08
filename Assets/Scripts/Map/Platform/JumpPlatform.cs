using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;

/// <summary>
/// 점프만 가능한 플랫폼
/// </summary>
public class JumpPlatform : MonoBehaviour
{
    private int _originJumpCount;
    private Player _player;
    private Jump _jumpAbility;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();
            _jumpAbility = (_player.ASC.IsGranted(AbilityKey.DoubleJump) ? _player.ASC.GetAbility(AbilityKey.DoubleJump) : _player.ASC.GetAbility(AbilityKey.Jump)) as Jump;
            
            // 무조건 점프 1번만 가능
            if (_jumpAbility != null)
            {
                _originJumpCount = _jumpAbility.GetJumpCount();
                _jumpAbility.SetJumpCount(1);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_jumpAbility != null)
            {
                // 플랫폼 이용하지 않고 그대로 지나갔을 때
                if (_jumpAbility.GetJumpCount() > 0 && _originJumpCount > 0)
                {
                    _jumpAbility.SetJumpCount(_originJumpCount);
                }
            }
        }
    }
}
