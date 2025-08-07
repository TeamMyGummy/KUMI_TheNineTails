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

    private float _dashPower;
    private float _dashTime;
    private float _delayTime;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);

        IsTickable = true;
        _animator = Actor.GetComponent<Animator>();
    }

    // 점프와 대쉬를 동시에 눌렀을 때 대각선으로 나간다는 문제가 있음
    protected override void Activate()
    {
        base.Activate();

        _rigid = Actor.GetComponent<Rigidbody2D>();
        _characterMovement = Actor.GetComponent<CharacterMovement>();
        _playerController = Actor.GetComponent<PlayerController>();

        InitDash();
        _animator.SetBool(_dashID, true);
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
        if(_dashTime < 0)
        {
            _rigid.velocity = Vector2.zero;
            _playerController.OnDisableAllInput();

            _delayTime -= Time.deltaTime;
            if( _delayTime < 0 )
            {
                this.DelayOneFrame().Forget();
                EndDash();
                _playerController.OnEnableAllInput();
            }
        }
    }

    private void InitDash()
    {
        _dashPower = 15.0f;
        _dashTime = _so.BlockTimer;
        _delayTime = 0.4f;
    }

    private void EndDash()
    {
        InitDash();
        _animator.SetBool(_dashID, false);
        //AbilityFactory.Instance.EndAbility(this);
    }

}
