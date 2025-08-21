using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;

public class FireToPlayer : BlockAbility<FireProjectileSO>
{
    protected FireProjectileSO _so;
    protected IMovement _movement;
    protected CharacterMovement _cm;
    protected MonsterMovement _move;
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as FireProjectileSO;
        _movement = actor.GetComponent<IMovement>();
        _cm = actor.GetComponent<CharacterMovement>();
        _move = actor.GetComponentInChildren<MonsterMovement>();
    }

    protected async override void Activate()
    {
        base.Activate();
        GameObject go = ResourcesManager.Instance.Instantiate(_so.projectile.gameObject);
        Vector2 direction = GetDirectionToPlayer();

        if (_so.isStoppWhileAttack)
        {
            _move?.SetPaused(true);
        }
        
        go.GetComponent<Projectile>().FireProjectile(Actor, direction);

        if (_so.isStoppWhileAttack)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
            _move?.SetPaused(false);
        }
    }
    
    private Vector2 GetDirectionToPlayer()
    {
        // 플레이어 오브젝트 찾기 (태그로 찾는 방법)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // 몬스터 위치에서 플레이어 위치로의 방향 계산
            Vector2 monsterPosition = Actor.transform.position;
            Vector2 playerPosition = (Vector2)player.transform.position + Vector2.up;
            Vector2 direction = (playerPosition - monsterPosition).normalized;
            
            return direction;
        }
        
        // 플레이어를 찾지 못한 경우 기본 방향 반환 (오른쪽)
        return _movement.Direction;
    }
}
