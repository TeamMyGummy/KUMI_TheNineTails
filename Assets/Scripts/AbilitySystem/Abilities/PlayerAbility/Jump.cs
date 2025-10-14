using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameAbilitySystem;

public class Jump : GameplayAbility, ITickable
{
    private Player _player;
    private Rigidbody2D _rigidBody;
    private CharacterMovement _characterMovement;
    private PlayerController _playerController;
    private JumpSO _jumpSO;

    private int _maxJumpCount;
    private int _remainJumpCount;
    private float _maxJumpPower;
    private float _jumpPower;
    private bool isJumpKeyDown;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        _rigidBody = Actor.GetComponent<Rigidbody2D>();
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerController = Actor.GetComponent<PlayerController>();
        _player = Actor.GetComponent<Player>();
        
        IsTickable = true;
        _jumpSO = (JumpSO) abilitySo;
        
        _maxJumpCount = _jumpSO.MaxJumpCount;
        _maxJumpPower = _jumpSO.JumpPower;
        _remainJumpCount = _maxJumpCount;
        _jumpPower = _maxJumpPower;
    }

    public override bool CanActivate()
    {
        if (_remainJumpCount > 0)
        {
            return true;
        }
        
        return false;
    }

    protected override void Activate()
    {
        _playerController.OnJumpCanceled += JumpCanceled;
        isJumpKeyDown = true;
        
        // Jump
        _remainJumpCount--;
        _characterMovement.Jump(_jumpPower);
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        // Extra Jump
        if (_rigidBody.velocity.y > 0.0f)
        {
            if (isJumpKeyDown)
            {
                ExtraJump();
            }
        }
        
        // 땅에 닿았을 때 점프 횟수 초기화
        if (_rigidBody.velocity.y <= 0.0f)
        {
            if (_characterMovement.CheckIsGround())
            {
                SoundManager.Instance.PlaySFX(SFXName.착지);
                ResetJumpCount();
                if (_playerController != null)
                {
                    this.DelayOneFrame().Forget();
                }
            }
        }
    }

    public void ExtraJump()
    {
        _rigidBody.AddForce(Vector2.up * (_jumpPower / 25), ForceMode2D.Impulse);
    }

    public void JumpCanceled()
    {
        isJumpKeyDown = false;
    }

    public int GetJumpCount()
    {
        return _remainJumpCount;
    }
    public void SetJumpCount(int count)
    {
        _remainJumpCount = count;
    }
    
    public void ResetJumpCount()
    {
        _remainJumpCount = _maxJumpCount;
    }

    public float GetMaxJumpPower()
    {
        return _maxJumpPower;
    }
    public void SetJumpPower(float power)
    {
        _jumpPower = power;
    }
    
    public void ResetJumpPower()
    {
        _jumpPower = _maxJumpPower;
    }

}

