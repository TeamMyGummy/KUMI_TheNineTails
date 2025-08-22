using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAbilitySystem;

public class ParryingHitbox : MonoBehaviour
{
    public event System.Action LiverExtraction; // 간 뺴기 스킬 활성화
    public bool OnEnableLiverExtraction = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Parrying()
    {
        Debug.Log("패링 성공");
        AbilitySystem asc;
        DomainFactory.Instance.GetDomain(DomainKey.Player, out asc);
        asc.ApplyGameplayEffect(asc, new InstantGameplayEffect("FoxFireGauge", 1));
        if (Mathf.Approximately(asc.Attribute.Attributes["FoxFireGauge"].CurrentValue.Value, asc.Attribute.Attributes["FoxFireGauge"].MaxValue))
        {
            asc.Attribute.Attributes["FoxFireCount"].CurrentValue.Value += 1;
            asc.Attribute.Attributes["FoxFireGauge"].Reset();
        }
    }
    
    public void StartLiverExtraction()
    {
        OnEnableLiverExtraction = true;
        LiverExtraction?.Invoke();
    }
}
