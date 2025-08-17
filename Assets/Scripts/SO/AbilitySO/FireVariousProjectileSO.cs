using GameAbilitySystem;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(menuName = "Ability/FireVariousProjectile")]
public class FireVariousProjectileSO : BlockAbilitySO
{
    public Projectile projectile;
    
    public int projectileCount = 15; // 3초 동안 발사할 총 투사체 개수
    public float totalDuration = 3f; // 투사체를 발사하는 데 걸리는 총 시간
    public Vector2 direction;
    public Vector2 delta;
}