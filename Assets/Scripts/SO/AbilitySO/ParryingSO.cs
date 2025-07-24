using GameAbilitySystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/GameplayAbility/Parrying")]
public class ParryingSO : GameplayAbilitySO
{
    public GameObject Hitbox;
    public RectTransform GaugeBar;

    public float FullGaugeDuration = 0.5f;
    public float PauseDuration = 0.2f;
    public float RecoveryDuration = 0.3f;
    public float EndDuration = 0.3f;
}
