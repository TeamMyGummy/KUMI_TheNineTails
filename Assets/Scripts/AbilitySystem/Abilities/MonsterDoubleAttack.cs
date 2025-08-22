using Cysharp.Threading.Tasks;
using System;

public class MonsterDoubleAttack : MonsterAttack
{
    protected override async void Activate()
    {
        base.Activate();
        if (_attackData == null) return;

        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        Attack();
    }
}

