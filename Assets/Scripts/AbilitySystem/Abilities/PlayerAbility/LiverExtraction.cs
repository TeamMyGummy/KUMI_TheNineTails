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
        
        _sequenceSO = abilitySo.skillSequence;
        //_task = new AbilityTask(actor, actor.GetComponentInChildren<Camera>(), _sequenceSO);
        _so = abilitySo as LiverExtractionSO;
        _hitbox = _so.Hitbox;
        
        _hitbox = ResourcesManager.Instance.Instantiate(_so.Hitbox, actor.transform);
        _hitbox.SetActive(false);
    }

    protected override void Activate()
    {
        base.Activate();
        
        IsUsingLiverExtraction = true;
        //_task.Execute();
        
        // Hitbox 설정
        if (_hitbox != null)
        {
            SpawnHitbox();
        }
        
        SkillTimer(1.0f).Forget();
    }
    
    public void SpawnHitbox()
    {
        _hitbox.SetActive(false);
        _hitbox.SetActive(true);
        _hitbox.transform.localPosition = Actor.GetComponent<SpriteRenderer>().flipX 
            ? new Vector2(_spawnPoint.x * (-2), _spawnPoint.y) 
            : new Vector2(_spawnPoint.x, _spawnPoint.y);
    }
    
    
    private async UniTask SkillTimer(float duration)
    {
        _skillTime = duration;
        while (_skillTime > 0.0f)
        {
            _skillTime -= Time.deltaTime;
            await UniTask.Yield();
        }
        
        IsUsingLiverExtraction = false;
        
        Actor.GetComponent<PlayerController>().OnDisableLiverExtraction();
        _hitbox.SetActive(false);
        //_task.Canceled();;
    }
}
