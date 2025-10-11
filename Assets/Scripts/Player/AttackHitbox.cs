    using System;
    using GameAbilitySystem;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using Unity.VisualScripting;

    public class AttackHitbox : MonoBehaviour
    {
        private GameObject _actor;
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
            if (collision.gameObject.CompareTag("BreakableWall"))
            {
                collision.gameObject.GetComponent<BreakableWall>().AttackCount();
                return;
            }
            
            // 데미지 처리
            AbilitySystem asc = collision.GetComponent<IAbilitySystem>().asc;
            Vector2 attackDirection = _actor.transform.position.x > collision.transform.position.x ? Vector2.left : Vector2.right; // 힘의 방향
            collision.GetComponent<Damageable>().GetDamage(asc, 10.0f, attackDirection);
            
            if (collision.CompareTag("Enemies"))
            {
                collision.GetComponent<MonsterMovement>().EnterOuchState();
            }
        
            // 공격 성공 시 Effect
            EffectManager.Instance.AttackEffect(effectSO, collision.transform.position, _effectPrefab);
        }
        

    }
