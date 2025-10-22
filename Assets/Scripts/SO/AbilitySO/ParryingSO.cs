using GameAbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Ability/Parrying")]
public class ParryingSO : GameplayAbilitySO
{
    public GameObject Hitbox;
    public RectTransform GaugeBar;

    public Vector3 HitboxOffset = new Vector3(0.7f, 0.7f);

    public float FullGaugeDuration = 0.5f;
    public float PauseDuration = 0.2f;
    public float RecoveryDuration = 0.3f;
    public float EndDuration = 0.3f;
}
