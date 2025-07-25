using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;

public class Player : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    
    private readonly int _verticalSpeedID = Animator.StringToHash("VerticalSpeed");
    private readonly int _runID = Animator.StringToHash("Run");
    private readonly int _isGroundID =  Animator.StringToHash("IsGround");
    private readonly int _wallClimbID =  Animator.StringToHash("WallClimb");
    private readonly int _isWallClimbingID =  Animator.StringToHash("IsWallClimbing");
    private readonly int _endWallClimbID =  Animator.StringToHash("EndWallClimb");
    
    // Start is called before the first frame update
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        _animator.SetBool(_runID, _characterMovement.GetCharacterDirection() != Vector2.zero);
        _animator.SetFloat(_verticalSpeedID, _rigidbody2D.velocity.y);
        _animator.SetBool(_isGroundID, _characterMovement.CheckIsGround());
        _animator.SetBool
        (
            _isWallClimbingID,
            _animator.GetBool(_wallClimbID) && _characterMovement.GetCharacterDirection().y != 0 ? true : false
        );
        
        if (_characterMovement.GetCharacterDirection().x != 0)
        {
          _spriteRenderer.flipX = _characterMovement.GetCharacterDirection().x > 0 ? false : true;  
        }
        
    }

    public void SetWallClimb(bool isWallClimbing)
    {
        _animator.SetBool(_wallClimbID, isWallClimbing);
    }

}
