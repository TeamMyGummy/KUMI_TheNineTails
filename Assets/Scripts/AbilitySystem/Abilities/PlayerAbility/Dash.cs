using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;

public class Dash : BlockAbility<BlockAbilitySO>, ITickable
{
    private Player _player;
    private Rigidbody2D _rigid;
    private CharacterMovement _characterMovement;
    private PlayerController _playerController;

    private DashSO _dashSO;
    private float _dashPower;
    private float _dashTime;
    private float _endDelayTime;
    private bool _canDash = true;
    private bool _endDash;
    private Vector2 _originVelocity;
    
    public static System.Action ResetDash;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        IsTickable = true;
        _dashSO = (DashSO) abilitySo;

        _player = Actor.GetComponent<Player>();
        _rigid = Actor.GetComponent<Rigidbody2D>();
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerController = Actor.GetComponent<PlayerController>();

        ResetDash += ResetDashCount;
    }

    public override bool CanActivate()
    {
        if (_characterMovement.CheckIsGround())
        {
            _canDash = true;
        }
        
        return _canDash && base.CanActivate();
    }

    protected override void Activate()
    {
        base.Activate();
        
        if (!_characterMovement.CheckIsGround())
        {
            // 공중에서 대쉬 한 번만 가능 
            _canDash = false;
        }
        
        StartDash();
        Vector2 currentPosition = _rigid.position;
        float dashDistance = (_dashPower / _rigid.mass) * _dashTime;

        _rigid.velocity = Vector2.zero;
        _rigid.AddForce(_characterMovement.GetCharacterSpriteDirection() * _dashPower, ForceMode2D.Impulse);         
    }

    public void Update()
    {

    }
    public void FixedUpdate()
    {
        _dashTime -= Time.deltaTime;
        if(_dashTime < 0 && !_endDash)
        {
            _rigid.velocity = Vector2.zero;
            _endDelayTime -= Time.deltaTime;
            if( _endDelayTime < 0 )
            {
                if (_characterMovement.CheckIsGround())
                    this.DelayOneFrame().Forget();
                EndDash();
            }
        }
        if (_characterMovement.CheckIsGround())
        {
            if (!_canDash)
            {
                this.DelayOneFrame().Forget(); 
                _canDash = true;
            }
        }
    }

    private void StartDash()
    {
        _dashPower = _dashSO.dashPower;
        _dashTime = _so.BlockTimer;
        _endDelayTime = _dashSO.endDelay;
        _originVelocity = _rigid.velocity;
        _rigid.gravityScale = 0;
        _endDash = false;
    }

    private void EndDash()
    {
        _rigid.velocity = _originVelocity;
        _rigid.gravityScale = _characterMovement.Gravity;
        _endDash = true;
    }

    private void ResetDashCount()
    {
        _canDash = true;
    }

}
