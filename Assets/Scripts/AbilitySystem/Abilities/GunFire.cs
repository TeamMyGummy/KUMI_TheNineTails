using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;

public class GunFire : BlockAbility<GunFireSO>
{
    protected GunFireSO _so;
    protected MonsterMovement _move;
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as GunFireSO;
        _move = actor.GetComponentInChildren<MonsterMovement>();
    }

    protected async override void Activate()
    {
        base.Activate();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        Vector2 startPos = Actor.transform.position;
        Vector2 endPos = (Vector2)player.transform.position + Vector2.up;

        if (_so.isStoppWhileAttack)
        {
            _move?.SetPaused(true);
        }
        
        Vector2 midPoint = (startPos + endPos) / 2;
        Vector2 directionToEnd = (endPos - startPos).normalized;

        Vector2 perpendicularDir = Vector2.Perpendicular(directionToEnd);

        for (int i = 0; i < _so.numberOfProjectiles; i++)
        {
            // 1. 중간 지점에서 각 투사체가 위치할 '산 정상(peak)' 지점을 계산
            //    spreadWidth에 따라 수직선 위에 투사체들을 분배
            float spreadAmount = (i / (float)(_so.numberOfProjectiles - 1) - 0.5f) * _so.spreadWidth;
            Vector2 peakPoint = midPoint + perpendicularDir * spreadAmount;

            // 2. 투사체가 정확히 peakPoint를 지나가도록 만드는 제어점을 역산
            //    (공식: ControlPoint = 2 * PeakPoint - MidPoint)
            Vector2 controlPoint = 2 * peakPoint - midPoint;

            // --- ---

            GameObject projectile = ResourcesManager.Instantiate(_so.projectile).gameObject;
            MoveProjectileAlongCurve(projectile, startPos, endPos, controlPoint).Forget();
        }
        
        if (_so.isStoppWhileAttack)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_so.stopDuration));
            _move?.SetPaused(false);
        }
        
    }

    private async UniTaskVoid MoveProjectileAlongCurve(GameObject projectile, Vector3 start, Vector3 end, Vector3 control)
    {
        if (projectile == null) return;
        
        float timer = 0f;
        projectile.transform.position = start;

        // SO에서 duration과 speedCurve 값을 가져와 사용합니다.
        while (timer < _so.duration)
        {
            if (projectile == null || !projectile.activeSelf) break;
            
            float timeRatio = Mathf.Clamp01(timer / _so.duration);
            float progress = _so.speedCurve.Evaluate(timeRatio);

            float u = 1 - progress;
            Vector3 newPos = u * u * start + 2 * u * progress * control + progress * progress * end;
            
            Vector3 direction = (newPos - projectile.transform.position).normalized;
            if (direction != Vector3.zero)
            {
                projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            }
            
            projectile.transform.position = newPos;

            await UniTask.Yield(PlayerLoopTiming.Update);
            timer += Time.deltaTime;
        }

        if (projectile != null)
        {
            ResourcesManager.Instance.Destroy(projectile);
        }
    }
}

