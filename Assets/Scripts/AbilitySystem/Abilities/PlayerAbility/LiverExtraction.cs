using System.Collections;
using System.Collections.Generic;
using GameAbilitySystem;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class LiverExtraction : BlockAbility<BlockAbilitySO>
{
    public bool IsUsingLiverExtraction { get; private set; }
    
    private AbilitySequenceSO _sequenceSO;
    private AbilityTask _task;
    private LiverExtractionSO _so;
    private GameObject _hitbox;

    private float _skillTime;
    private readonly Vector2 _spawnPoint = new (0.55f, 0.98f);
    
    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        
        _so = abilitySo as LiverExtractionSO;
        _hitbox = _so.Hitbox;
        
        _hitbox = ResourcesManager.Instance.Instantiate(_so.Hitbox, actor.transform);
        _hitbox.SetActive(false);
    }

    protected override void Activate()
    {
        base.Activate();
        
        IsUsingLiverExtraction = true;
        // Hitbox 설정
        if (_hitbox != null)
        {
            // 애니메이션 이벤트로 대체함 -> Player에서 실행
            //SpawnHitbox();
        }
    }
    
    public void SpawnHitbox()
    {
        _hitbox.SetActive(false);
        _hitbox.SetActive(true);
        _hitbox.transform.localPosition = Actor.GetComponent<SpriteRenderer>().flipX 
            ? new Vector2(_spawnPoint.x * (-2), _spawnPoint.y) 
            : new Vector2(_spawnPoint.x, _spawnPoint.y);
    }

    public void EndAbility()
    {
        IsUsingLiverExtraction = false;
        RecoverPlayerHp(1f);
        Actor.GetComponent<PlayerController>().OnDisableLiverExtraction();
        _hitbox.SetActive(false);
    }

    private void RecoverPlayerHp(float healAmount)
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out AbilitySystem asc);
        if (asc == null) return;

        GameplayAttribute att = asc.Attribute;

        InstantGameplayEffect effect = new("HP", healAmount);
        effect.Apply(att);
    }
}
