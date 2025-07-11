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
    private JumpSO _jumpSO;

    private int _maxJumpCount;
    //private int _jumpCount;
    private float _jumpPower;
    private bool isJumpKeyDown;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        IsTickable = true;
        _jumpSO = (JumpSO) abilitySo;
        _maxJumpCount = _jumpSO.MaxJumpCount;
        _jumpPower = _jumpSO.JumpPower;
    }

    protected override void Activate()
    {
        _rigidBody = Actor.GetComponent<Rigidbody2D>();
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerController = Actor.GetComponent<PlayerController>();    
        
        PlayerController.OnJumpCanceled += JumpCanceled;
        isJumpKeyDown = true;

        if (_playerController.JumpCount < _maxJumpCount)
        {
            _playerController.JumpCount++;
            _characterMovement.Jump(_jumpPower);
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
                _playerController.JumpCount = 0;
                if (_playerController != null)
                {
                    this.DelayOneFrame().Forget();
                    _playerController.OnEnableJump();
                }
            }
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

