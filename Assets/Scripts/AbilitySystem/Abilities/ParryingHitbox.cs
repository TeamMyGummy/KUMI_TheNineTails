using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;
using Cysharp.Threading.Tasks;

public class ParryingHitbox : MonoBehaviour
{
    private GameObject _actor;
    
    public EffectSO effectSO;
    private GameObject _effectPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        _actor = GameObject.FindWithTag("Player");
        
        // Effect 초기화
        _effectPrefab = ResourcesManager.Instance.Instantiate(effectSO.hitEffectPrefab, _actor.transform);
        _effectPrefab.SetActive(false);
    }

    public void Parrying()
    {
        Debug.Log("패링 성공");
        
        // 사운드
        SoundManager.Instance.PlaySFX(SFXName.패링);
        
        // 패링 이펙트 생성
        EffectManager.Instance.AttackEffect(effectSO, _actor.transform.position, _effectPrefab);
        
        // 여우불 게이지 증가
        AbilitySystem asc;
        DomainFactory.Instance.GetDomain(DomainKey.Player, out asc);
        asc.ApplyGameplayEffect(asc, new InstantGameplayEffect("FoxFireGauge", 1));
        if (Mathf.Approximately(asc.Attribute.Attributes["FoxFireGauge"].CurrentValue.Value, asc.Attribute.Attributes["FoxFireGauge"].MaxValue))
        {
            asc.Attribute.Attributes["FoxFireCount"].CurrentValue.Value += 1;
            asc.Attribute.Attributes["FoxFireGauge"].Reset();
        }
    }
    
    public async UniTask StartLiverExtraction()
    {
        // 사운드
        SoundManager.Instance.PlaySFX(SFXName.패링);
        
        // 패링 이펙트 생성
        //EffectManager.Instance.AttackEffect(effectSO, _actor.transform.position, _effectPrefab);

        _actor.GetComponent<PlayerController>().OnEnableLiverExtraction();
        EffectManager.Instance.SlowMotionTask(0.2f, 1.0f).Forget();
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        
        _actor.GetComponent<PlayerController>().OnDisableLiverExtraction();
        Debug.Log("인풋 막힘");
    }
}
