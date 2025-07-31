using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;
/*
 * 플레이어의 기본 정보를 관리하는 파일입니다.
 * 변하지 않는 변수, 설정값 등
 */
public class Player : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private PlayerController _playerController;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    
    private readonly int _verticalSpeedID = Animator.StringToHash("VerticalSpeed");
    private readonly int _runID = Animator.StringToHash("Run");
    private readonly int _isGroundID =  Animator.StringToHash("IsGround");
    private readonly int _wallClimbID =  Animator.StringToHash("WallClimb");
    private readonly int _isWallClimbingID =  Animator.StringToHash("IsWallClimbing");
    private readonly int _endWallClimbID =  Animator.StringToHash("EndWallClimb");

    private bool _canFlip;
    
    // Start is called before the first frame update
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _canFlip = true;
        WallClimb.WallClimbEndAnimation += EndWallClimb;
    }

    void FixedUpdate()
    {
        _animator.SetBool(_runID, _characterMovement.GetCharacterDirection() != Vector2.zero);
        _animator.SetFloat(_verticalSpeedID, _rigidbody2D.velocity.y);
        _animator.SetBool(_isGroundID, _characterMovement.CheckIsGround());

        SetWallClimb(_characterMovement.CheckIsWallClimbing());
        _animator.SetBool
        (
            _isWallClimbingID,
            _animator.GetBool(_wallClimbID) && _characterMovement.GetCharacterDirection().y != 0 ? true : false
        );
        
        if (_characterMovement.GetCharacterDirection().x != 0 && _canFlip)
        {
          _spriteRenderer.flipX = _characterMovement.GetCharacterDirection().x > 0 ? false : true;  
        }
        
    }

    public void SetWallClimb(bool wallClimb)
    {
        _animator.SetBool(_wallClimbID, wallClimb);
        _canFlip = !wallClimb;
    }

    private void EndWallClimb()
    {
        _animator.SetTrigger(_endWallClimbID);
    }

}
