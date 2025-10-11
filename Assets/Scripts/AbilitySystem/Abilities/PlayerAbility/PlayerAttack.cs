using System;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerAttack : BlockAbility<BlockAbilitySO>
{
    private PlayerAttackSO _so;
    private GameObject _hitbox;
    
    private readonly Vector2 _spawnPoint = new (0.55f, 0.98f);
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        _so = abilitySo as PlayerAttackSO;
        
        // Hitbox 초기화
        _hitbox = ResourcesManager.Instance.Instantiate(_so.Hitbox, actor.transform);
        _hitbox.SetActive(false);
    }
    

    /// <summary>
    /// 실제 Ability 실행부
    /// </summary>
    protected override void Activate() 
    {
        base.Activate();

        if (_hitbox != null)
        {
            // Hitbox 생성
            SpawnHitbox();
        }
    }
    
    private async UniTask SpawnHitbox()
    {
        _hitbox.SetActive(true);
 
        _hitbox.transform.localPosition = Actor.GetComponent<SpriteRenderer>().flipX 
            ? new Vector2(_spawnPoint.x * (-2), _spawnPoint.y) 
            : new Vector2(_spawnPoint.x, _spawnPoint.y);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        
        _hitbox.SetActive(false);
    }
}
