using System;
using GameAbilitySystem;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string parryLayerName = "Parrying";
    [SerializeField] private float damage = 1f;

    private BoxCollider2D _boxCollider;
    private readonly Collider2D[] _results = new Collider2D[1];

    private GameObject _attacker;
    public GameObject liverPrefab;
    private GameObject liverObject;
    
    public EffectSO effectSO;
    private GameObject _effectPrefab;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // 간 생성
        if (liverPrefab != null && _attacker != null)
        {
            liverObject = ResourcesManager.Instance.Instantiate(liverPrefab, _attacker.transform);
            liverObject.SetActive(false);
        }
        
        // Effect 초기화
        if (effectSO != null && _attacker != null)
        {
            _effectPrefab = ResourcesManager.Instance.Instantiate(effectSO.hitEffectPrefab, _attacker.transform);
            _effectPrefab.SetActive(false);
        }
    }

    public void SetAttacker(GameObject attacker)
    {
        _attacker = attacker;
    }

    private void DeactivateLiverObject()
    {
        liverObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_boxCollider == null) return;
        
        // 공격 성공 시 Effect
        if(_effectPrefab != null && effectSO != null)
            EffectManager.Instance.AttackEffect(_attacker, effectSO, other.bounds.center, _effectPrefab);
        
        Vector2 boxCenter = (Vector2)transform.position + _boxCollider.offset;
        Vector2 boxSize = _boxCollider.size;
        float angle = 0f;

        int parryLayer = LayerMask.NameToLayer(parryLayerName);
        if (parryLayer != -1)
        {
            int parryLayerMask = 1 << parryLayer;

            var size = Physics2D.OverlapBoxNonAlloc(boxCenter, boxSize, angle, _results, parryLayerMask);
            if (size > 0)
            {
                // 플레이어 패링 성공
                if (FindAnyObjectByType<DoParrying>() != null)
                    FindAnyObjectByType<DoParrying>().RecordParrySuccess();
                _attacker?.GetComponent<Monster>()?.OnParried();
                Monster monster = _attacker?.GetComponent<Monster>();
                if (monster != null && 
                    monster.asc.Attribute.Attributes["HP"].CurrentValue.Value <=
                    monster.asc.Attribute.Attributes["HP"].MaxValueRP.Value * 0.5f)
                {
                    // 간 빼기 스킬 활성화
                    ParryingHitbox ph = other.GetComponent<ParryingHitbox>();
                    if (ph != null)
                    {
                        ph.StartLiverExtraction();
                        
                        // 간 보이게 하기
                        if (liverObject != null)
                        {
                            liverObject.SetActive(true);
                            liverObject.transform.position = _attacker.GetComponent<Collider2D>().bounds.center;
                            ph.OnLiverExtractionEnded -= DeactivateLiverObject;
                            ph.OnLiverExtractionEnded += DeactivateLiverObject;
                        }
                    }
                }
                else
                {
                    other.GetComponent<ParryingHitbox>()?.Parrying();
                }
                return;
            }
        }

        // 플레이어 피격&넉백
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        if (_attacker != null)
        {
            // 몬스터가 있을 때 : 몬스터 위치 기준
            Vector2 attackDirection = playerTransform.position.x > _attacker.transform.position.x ? Vector2.right : Vector2.left;
            other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, damage, attackDirection);              
        }
        else
        {
            // 몬스터가 없을 때 (+ 투사체일 때) : HitBox 위치 기준
            Vector2 attackDirection = playerTransform.position.x > transform.position.x ? Vector2.right : Vector2.left;
            other.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, damage, attackDirection);
        }
    }
}
