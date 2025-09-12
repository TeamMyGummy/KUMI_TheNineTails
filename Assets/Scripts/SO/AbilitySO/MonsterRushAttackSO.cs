using UnityEngine;
using GameAbilitySystem;

[CreateAssetMenu(menuName = "Ability/MonsterRushAttackSO")]
public class MonsterRushAttackSO : MonsterAttackSO  
{
    [Header("돌진 관련")] 
    /*[SerializeField] private float rushChargeTime = 1f;*/     // 돌진 전 차징 시간 (플래시 시간)
    [SerializeField] private float rushSpeed = 20f;         // 돌진 속도
    [SerializeField] private float rushDistance = 6f;  //돌진 거리
    [SerializeField] private float betweenRushAttackDelay = 1f; //돌진과 공격 사이 딜레이 
    
    /*public float RushChargeTime => rushChargeTime;*/
    public float RushSpeed => rushSpeed;
    public float RushDistance => rushDistance;
    public float BetweenRushAttackDelay => betweenRushAttackDelay;
}