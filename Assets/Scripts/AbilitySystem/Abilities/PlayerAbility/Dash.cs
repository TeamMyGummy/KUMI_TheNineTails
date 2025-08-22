using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;

public class Dash : BlockAbility<BlockAbilitySO>, ITickable
{
    private Rigidbody2D _rigid;
    private CharacterMovement _characterMovement;
    private PlayerController _playerController;
    private Animator _animator;
    private readonly int _dashID =  Animator.StringToHash("Dash");

    private DashSO _dashSO;
    private float _dashPower;
    private float _dashTime;
    private float _endDelayTime;
    private bool _canDash = true;
    private bool _endDash;
    private Vector2 _originVelocity;

    private AbilitySequenceSO _sequenceSO;
    private AbilityTask _task;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        IsTickable = true;
        _dashSO = (DashSO) abilitySo;
        
        _animator = Actor.GetComponent<Animator>();
        _rigid = Actor.GetComponent<Rigidbody2D>();
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerController = Actor.GetComponent<PlayerController>();
        
        // Task
        _sequenceSO = abilitySo.skillSequence;
        _task = new AbilityTask(actor, actor.GetComponentInChildren<Camera>(), _sequenceSO);
    }
    
    protected override void Activate()
    {
        base.Activate();

        if (_characterMovement.CheckIsGround())
        {
            _canDash = true;
        }
        
        if (_canDash)
        {
            if (!_characterMovement.CheckIsGround())
            {
                _canDash = false;
            }
            StartDash();
            _task.Execute();
            //_animator.SetBool(_dashID, true);
            Vector2 currentPosition = _rigid.position;
            float dashDistance = (_dashPower / _rigid.mass) * _dashTime;
   
            _rigid.velocity = Vector2.zero;
            _rigid.AddForce(_characterMovement.GetCharacterSpriteDirection() * _dashPower, ForceMode2D.Impulse);         
        }
 
        
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
            _playerController.OnDisableAllInput();

            _endDelayTime -= Time.deltaTime;
            if( _endDelayTime < 0 )
            {
                if (_characterMovement.CheckIsGround())
                    this.DelayOneFrame().Forget();
                EndDash();
                _playerController.OnEnableAllInput();
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
        _task.Canceled();
        _rigid.velocity = _originVelocity;
        _rigid.gravityScale = _characterMovement.Gravity;
        _endDash = true;
        //_animator.SetBool(_dashID, false);
        //AbilityFactory.Instance.EndAbility(this);
    }

}
