using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditorInternal;


public class PlayerAttack : BlockAbility, ITickable
{
    private Animator _animator;
    private AnimatorStateInfo _animatorStateInfo;
    private AttackRange _attackRange;
    
    private readonly String[] _animationNames = new [] {"AirSlash", "AirSlashUp", "AirSlashDown"};
    private String _currentAnimationName;
    private readonly String _parameterName = "AttackCount";
    private int _parameterID;
    private int _attackCount;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        IsTickable = true;
        _attackCount = 0;
        
        _animator = Actor.GetComponent<Animator>();
        _parameterID = Animator.StringToHash(_parameterName);

        _attackRange = Actor.GetComponentInChildren<AttackRange>();
    }
    
    /// <summary>
    /// 실제 Ability 실행부
    /// </summary>
    protected override void Activate() 
    {
        base.Activate();

        if (_attackRange != null && _animator != null)
        {
            switch (_attackCount)
            {
                case 0:
                    Attack(1);
                    break;
                case 1:
                    Attack(2);
                    break;
                case 2:
                    Attack(3);
                    break;
                default:
                    break;
            } 
        }
        
        //EndSkill().Forget();
    }

    public void Update()
    {
        _animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (_animatorStateInfo.IsName(_currentAnimationName) && _animatorStateInfo.normalizedTime >= 1)
        {
            EndAttack();
            EndSkill().Forget();
        }
    }

    public void FixedUpdate()
    {
        
    }

    public async UniTask EndSkill()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_so.BlockTimer), DelayType.DeltaTime);
        AbilityFactory.Instance.EndAbility(this);
    }
    
    private void Attack(int attackCount)
    {
        // Collider 설정
        _attackRange.SpawnAttackRange();
        _attackRange.EnableAttackCollider(false);
        _attackRange.EnableAttackCollider(true);
        
        // Animation 설정
        _animator.SetInteger(_parameterID, attackCount);
        _currentAnimationName =  _animationNames[attackCount - 1];
        if (attackCount == 3)
        {
            ResetAttackCount();
            return;
        }
        _attackCount++;
    }
    
    private void EndAttack()
    {
        _attackRange.EnableAttackCollider(false);
        _animator.SetInteger(_parameterID, 0);
        ResetAttackCount();
    }

    private void ResetAttackCount()
    {
        _attackCount = 0;
    }
}
