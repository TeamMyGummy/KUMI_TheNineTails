using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;

public class FireSonicWave : BlockAbility<FireSonicWaveSO>
{
    protected FireSonicWaveSO _so;
    protected IMovement _movement;
    protected CharacterMovement _cm;
    protected MonsterMovement _move;
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as FireSonicWaveSO;
        _movement = actor.GetComponent<IMovement>();
        _cm = actor.GetComponent<CharacterMovement>();
        _move = actor.GetComponentInChildren<MonsterMovement>();
    }

    protected async override void Activate()
    {
        base.Activate();
        GameObject go = ResourcesManager.Instance.Instantiate(_so.SonicWave.gameObject);
        Vector2 direction = GetDirectionToPlayer();

        if (_so.isStopWhileAttack)
        {
            _move?.SetPaused(true);
            Debug.Log("pause");
        }
        
        go.GetComponent<SonicWave>().FireProjectile(Actor, direction);

        if (_so.isStopWhileAttack)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
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
