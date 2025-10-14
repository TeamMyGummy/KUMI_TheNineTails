    using System;
    using System.Collections.Generic;
    using GameAbilitySystem;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using Unity.VisualScripting;

    public class AttackHitbox : MonoBehaviour
    {
        private GameObject _actor;
        private HashSet<Collider2D> _hitTargets = new HashSet<Collider2D>();    // 중복 타격 방지

        public float damage;
        public EffectSO effectSO;
        private GameObject _effectPrefab;

        private void Start()
        {
            _actor = GameObject.FindWithTag("Player");
            
            // Effect 초기화
            _effectPrefab = ResourcesManager.Instance.Instantiate(effectSO.hitEffectPrefab, _actor.transform);
            _effectPrefab.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hitTargets.Contains(collision)) return;
            _hitTargets.Add(collision);
            
            if (collision.gameObject.CompareTag("BreakableWall"))
            {
                collision.gameObject.GetComponent<BreakableWall>().AttackCount();
                ResetHitTargets();
                return;
            }
            
            if (collision.gameObject.CompareTag("BreakableBox"))
            {
                collision.gameObject.GetComponent<HonbulBox>().AttackCount();
                ResetHitTargets();
                return;
            }
            
            // 데미지 처리
            AbilitySystem asc = collision.GetComponent<IAbilitySystem>().asc;
            Vector2 attackDirection = _actor.transform.position.x > collision.transform.position.x ? Vector2.left : Vector2.right; // 힘의 방향
            collision.GetComponent<Damageable>().GetDamage(asc, damage, attackDirection);
            
            if (collision.CompareTag("Enemies"))
            {
                collision.GetComponent<MonsterMovement>().EnterOuchState();
            }
        
            // 공격 성공 시 Effect
            EffectManager.Instance.AttackEffect(effectSO, collision.transform.position, _effectPrefab);
        }

        /// <summary>
        /// 중복 타격 방지용 HashSet 초기화
        /// Object Pooling으로 Hitbox 생성 시 꼭 호출해줘야 함
        /// </summary>
        public void ResetHitTargets()
        {
            _hitTargets.Clear();
        }
        

    }
