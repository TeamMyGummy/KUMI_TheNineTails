using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCollider : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionDistance = 2f;
    [SerializeField] private LayerMask playerLayerMask = -1;
    
    private MovingPlatform _mp;
    private Collider2D _collider;
    private CharacterMovement _playerMovement;
    private Transform _playerTransform;
    
    private bool _isPlayerNearby = false;
    private float _lastUpdateTime;
    private const float UpdateInterval = 0.1f;
        
        private void Awake()
        {
            _mp = GetComponentInParent<MovingPlatform>();
            _collider = GetComponent<Collider2D>();
            
            if (_mp == null)
            {
                Debug.LogError("[MovingPlatformSprite] MovingPlatform component == null");
            }
            
            if (_collider == null)
            {
                Debug.LogError("[MovingPlatformCollider] Collider2D component == null");
            }
        }

        private void Start()
        {
            _collider.isTrigger = true;
            
            // 플레이어 캐싱...이 낫겠지?
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
                _playerMovement = player.GetComponent<CharacterMovement>();
            }
        }
        
        private void Update()
        {
            if (Time.time - _lastUpdateTime < UpdateInterval) return; 
            _lastUpdateTime = Time.time;
        
            UpdateTriggerState();
        }
        
        /// <summary>
        /// 플레이어가 이동플랫폼 가까워지면 공중에 있는지 확인 --> Trigger on
        /// </summary>
        private void UpdateTriggerState()
        {
            if (_playerTransform == null || _playerMovement == null) return;
            
            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
            _isPlayerNearby = distanceToPlayer <= detectionDistance;
        
            if (_isPlayerNearby)
            {
                bool shouldBeTrigger = _playerMovement.CheckIsGround();
                
                /*// 플레이어가 플랫폼 위에서 아래로 이동하는 경우: 플랫폼 통과x
                if (IsPlayerAbovePlatform())
                {
                    shouldBeTrigger = false;
                }*/
                
                _collider.isTrigger = shouldBeTrigger;
            }
        }
        
        // TODO: 근데 이거 필요해?? 중복이면 지우기 
        /// <summary>
        /// 플레이어가 플랫폼 위에 있는지 확인
        /// </summary>
        private bool IsPlayerAbovePlatform()
        {
            return _playerTransform.position.y > transform.position.y + _collider.bounds.size.y * 0.5f;
        }
    
        /// <summary>
        /// 이동플랫폼-플레이어 충돌 판정 --> 플레이어가 이동플랫폼 따라갈지 여부 결정
        /// 플랫폼 통과여부 결정
        /// </summary>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _mp.SetPlayer(other.transform);
            }
            
            // 끝부분 도착했을 때 플레이어 미끄러지거나 튀어오르지 않게 보정
            /*if (_mp.IsWaiting)
            {
                _mp.ApplyPlatformVelocity();
            }*/
        }
        
        /// <summary>
        /// 플레이어가 바닥에 있으면 플랫폼 통과
        /// </summary>
        /*private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (_playerMovement != null)
                {
                    if (_playerMovement.CheckIsGround())
                        _collider.isTrigger = true;  
                    else
                        _collider.isTrigger = false;
                }
            }
        }*/
        
        /*private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (_playerMovement != null)
                {
                    if (_playerMovement.CheckIsGround())
                        _collider.isTrigger = true;  
                    else
                        _collider.isTrigger = false;
                }
            }
        }*/
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _collider.isTrigger = false;
                _mp.RemovePlayer();
                _playerMovement = null;
            }
        }
        
        /*private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _collider.isTrigger = false;
                _mp.RemovePlayer();
                _playerMovement = null;
            }
        }*/
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionDistance);
        
            if (_collider != null)
            {
                Gizmos.color = _collider.isTrigger ? Color.green : Color.red;
                Gizmos.DrawWireCube(transform.position, _collider.bounds.size);
            }
        }
}
