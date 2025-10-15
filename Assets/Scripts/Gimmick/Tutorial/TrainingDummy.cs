using UnityEngine;
using GameAbilitySystem;
using R3; // R3 라이브러리를 사용하기 위해 필수입니다.
using System; // IDisposable을 사용하기 위해 필요합니다.

[RequireComponent(typeof(AbilitySystem))]
public class TrainingDummy : MonoBehaviour
{
    private AbilitySystem asc;
    
    // 구독 정보를 저장하기 위한 변수. 구독을 취소(해제)할 때 사용됩니다.
    private IDisposable healthSubscription;

    void Start()
    {
        asc = GetComponent<AbilitySystem>();

        // "HP" 속성이 있는지 확인하고, 있다면 해당 속성의 CurrentValue를 구독합니다.
        if (asc.Attribute.Attributes.TryGetValue("HP", out var hpAttribute))
        {
            // hpAttribute.CurrentValue가 바뀔 때마다 HandleHealthChange 메소드를 호출하도록 등록(구독)합니다.
            // 구독 정보는 healthSubscription 변수에 저장됩니다.
            healthSubscription = hpAttribute.CurrentValue.Subscribe(HandleHealthChange);
        }
        else
        {
            Debug.LogWarning("TrainingDummy: 'HP' 속성을 찾을 수 없습니다!", this);
        }
    }

    private void OnDestroy()
    {
        // 이 오브젝트가 파괴될 때, 등록했던 구독을 반드시 해제해야 합니다.
        // 그렇지 않으면 메모리 누수가 발생할 수 있습니다.
        healthSubscription?.Dispose();
    }

    /// <summary>
    /// HP의 ReactiveProperty 값이 변경될 때마다 호출되는 메소드입니다.
    /// </summary>
    /// <param name="newHealth">새롭게 변경된 HP 값</param>
    private void HandleHealthChange(float newHealth)
    {
        // HP가 0 이하로 떨어졌는지 확인합니다.
        if (newHealth <= 0)
        {
            Debug.Log("연습용 봇의 체력이 0이 되어 즉시 회복합니다.");
            ResetHealth();
        }
    }

    /// <summary>
    /// 체력을 최대치로 회복시킵니다.
    /// </summary>
    private void ResetHealth()
    {
        if (asc.Attribute.Attributes.TryGetValue("HP", out var hpAttribute))
        {
            // Attribute 클래스에 있는 SetCurrentValue 메소드를 사용하여
            // 현재 체력을 최대 체력(MaxValue)으로 설정합니다.
            hpAttribute.SetCurrentValue(hpAttribute.MaxValue);
        }
    }
}