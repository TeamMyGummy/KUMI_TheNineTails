using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/GunFire")]
public class GunFireSO : BlockAbilitySO
{
    [Header("Projectile Settings")]
    public Projectile projectile;
    
    [Header("Attack Behavior")]
    public bool isStoppWhileAttack = true; // 공격 중 멈춤 여부
    public float stopDuration = 0.8f;      // 멈춤 시간

    [Header("Path Settings")]
    public int numberOfProjectiles = 8;
    // 변경: fanAngle, arcHeight -> spreadWidth
    public float spreadWidth = 10f; // 중간 지점에서 투사체들이 정렬할 선의 총 길이
    public float duration = 1.2f;
    public AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
}
