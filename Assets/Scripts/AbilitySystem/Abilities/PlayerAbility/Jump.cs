using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameAbilitySystem;

public class Jump : GameplayAbility, ITickable
{
    private Rigidbody2D _rigidBody;
    private CharacterMovement _characterMovement;
    private PlayerController _playerController;
    private Animator _animator;
    private AnimatorStateInfo _animatorStateInfo;
    private JumpSO _jumpSO;

    private int _maxJumpCount;
    private int _jumpCount;
    private float _jumpPower;
    private bool isJumpKeyDown;
    private readonly int _parameterID = Animator.StringToHash("Jump");
    private readonly string _animationName = "Jump";

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        _rigidBody = Actor.GetComponent<Rigidbody2D>();
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerController = Actor.GetComponent<PlayerController>();
        _animator = Actor.GetComponent<Animator>();
        
        IsTickable = true;
        _jumpSO = (JumpSO) abilitySo;
        _maxJumpCount = _jumpSO.MaxJumpCount;
        _jumpPower = _jumpSO.JumpPower;
        _jumpCount = 0;
    }

    protected override void Activate()
    {
        _playerController.OnJumpCanceled += JumpCanceled;
        isJumpKeyDown = true;

        if (_jumpCount < _maxJumpCount || _characterMovement.CheckIsClimbing())
        {
            _jumpCount++;
            _characterMovement.Jump(_jumpPower);
            _animator.SetBool(_parameterID, false);
            _characterMovement.SetIsJumping(true);
        }
        else
        {
            _playerController.OnDisableJump();
        }
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        //Jump
        if (_rigidBody.velocity.y > 0.0f)
        {
            if (isJumpKeyDown)
            {
                ExtraJump();
            }
        }

        if (_rigidBody.velocity.y <= 0.0f)
        {
            if (_characterMovement.CheckIsGround())
            {
                _jumpCount = 0;
                _characterMovement.SetIsJumping(false);
                if (_playerController != null)
                {
                    this.DelayOneFrame().Forget();
                    _playerController.OnEnableJump();
                }
            }
        }
        
        _animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (_animatorStateInfo.IsName(_animationName) && _animatorStateInfo.normalizedTime >= 1)
        {
            _characterMovement.SetIsJumping(false);
        }
    }

    public void ExtraJump()
    {
        _rigidBody.AddForce(Vector2.up * (_jumpPower / 50), ForceMode2D.Impulse);
    }

    public void JumpCanceled()
    {
        isJumpKeyDown = false;
    }

}

