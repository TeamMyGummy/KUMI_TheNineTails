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
            AttackEffect(collision);
        }
        
        private void AttackEffect(Collider2D collision)
        {
            // Camera Shake
            if (effectSO.useCameraShake)
            {
                CameraManager.Instance.Shake(effectSO.shakeIntensity, effectSO.shakeDuration);
            }

            // Effect
            if (effectSO.hitEffectPrefab != null)
            {
                SpawnEffectTask(collision.transform.position, effectSO.effectDuration).Forget();
            }
            
            // Input
            if (effectSO.useBlockInput)
            {
                DisableInputTask(effectSO.blockInputDuration).Forget();
            }
            //SlowMotionTask(0.05f, 0.1f).Forget();
        }
    
        // 이펙트 관련 비동기 함수
        // 양이 많아지면 따로 빼겠음(10/10)
        private async UniTask SlowMotionTask(float duration, float timeScale)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f)); 
        
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * timeScale; // 물리 업데이트도 조정
        
            await UniTask.Delay(TimeSpan.FromSeconds(duration)); // 실제 시간 기준
        
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        private async UniTask SpawnEffectTask(Vector2 position, float duration)
        {
            _effectPrefab.SetActive(true);
            _effectPrefab.transform.position = position;
        
            await UniTask.Delay(TimeSpan.FromSeconds(duration)); // 실제 시간 기준
            
            _effectPrefab.SetActive(false);
        }

        private async UniTask DisableInputTask(float duration)
        {
            _actor.GetComponent<PlayerController>().OnDisableMove();
        
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
        
            _actor.GetComponent<PlayerController>().OnEnableMove();
        }
    }
