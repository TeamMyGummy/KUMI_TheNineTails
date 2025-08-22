using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MonsterDoubleAttack : MonsterAttack
{
    protected override bool UseBasicAttack => false;
    protected override async void Activate()
    {
        if (_attackData == null) return;
        
        // 스킬 사용시간 계산해서 그동안 다른 스킬 막기 (BlockAbility에 알려줄 시간 계산)
        float block =
            Mathf.Max(0f, _attackData.PreDelay) +
            Mathf.Max(0f, _attackData.ActiveTime) +  
            Mathf.Max(0f, _attackData.BetweenAttackDelay) +          
            Mathf.Max(0f, _attackData.ActiveTime) +  
            Mathf.Max(0f, _attackData.PostDelay);    
        _attackData.BlockTimer = block;      
        
        base.Activate();
        
        
        _movement?.SetPaused(true); // 이동 멈추기 
        try
        {
            if (_attackData.PreDelay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.PreDelay), delayType: DelayType.DeltaTime);

            Attack(); // 1타

            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.ActiveTime), delayType: DelayType.DeltaTime);

            await UniTask.Delay(TimeSpan.FromSeconds(_attackData.BetweenAttackDelay), delayType: DelayType.DeltaTime);

            Attack(); // 2타

            if (_attackData.ActiveTime > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.ActiveTime), delayType: DelayType.DeltaTime);

            if (_attackData.PostDelay > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_attackData.PostDelay), delayType: DelayType.DeltaTime);
        }
        finally
        {
            _movement?.SetPaused(false); // 이동 잠금 해제 
        }
    }
}