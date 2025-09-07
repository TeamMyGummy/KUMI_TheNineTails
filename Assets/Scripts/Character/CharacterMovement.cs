using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _sprite;

    [SerializeField] [Range(0.0f, 10.0f)] private float speed;
    [SerializeField] [Range(0.0f, 10.0f)] private float acceleration; // 가속도
    [SerializeField] [Range(0.0f, 10.0f)] private float deceleration;  // 감속도
    [SerializeField] [Range(0.0f, 3.0f)] private float gravity;
    [SerializeField] private LayerMask groundMask;  // 땅으로 인식되는 Layer
    
    private Vector2 _nextDirection;
    private Vector2 _currentVelocity; 
    private bool _isGround;
    private bool _isJumping;
    private bool _isWallClimbing;
    private bool _isRopeClimbing;
    private bool _isKnockedBack;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _rigidBody.gravityScale = gravity;
    }

    private void FixedUpdate()
    {
        _isGround = CheckIsGround();
        
        if (_isGround || _rigidBody.gravityScale == 0)
        {
            // Move
            // 땅에 있을 때: 즉시 반응
            _currentVelocity = _nextDirection * speed;
        }
        else
        {
            // Jump
            // 공중에 있을 때
            Vector2 targetVelocity = _nextDirection * speed;
            
            if (_nextDirection != Vector2.zero)
            {
                // 가속
                _currentVelocity = Vector2.MoveTowards(_currentVelocity, targetVelocity, 
                    acceleration * Time.fixedDeltaTime);
            }
            else
            {
                // 감속
                _currentVelocity = Vector2.MoveTowards(_currentVelocity, Vector2.zero, 
                    deceleration * Time.fixedDeltaTime);
            }
        }
        
        if (_currentVelocity.magnitude > 0.01f)
        {
            Vector2 currMove = _rigidBody.position;
            Vector2 nextMove = _currentVelocity * Time.fixedDeltaTime;
            _rigidBody.position = currMove + nextMove;
        }
    }

    /// <summary>
    /// 캐릭터가 실제 이동하는 방향(인풋)
    /// </summary>
    /// <returns>Vector2 방향</returns>
    public Vector2 GetCharacterDirection()
    {
        return _nextDirection;
    }

    /// <summary>
    /// Sprite가 바라보는 x방향
    /// (1, 0) -> 오른쪽을 보고 있음
    /// (-1, 0) -> 왼쪽을 보고 있음
    /// </summary>
    /// <returns>Vector2 Sprite 방향</returns>
    public Vector2 GetCharacterSpriteDirection()
    {
        return new Vector2(_sprite.flipX ? -1 : 1, 0);
    }

    public Vector2 GetVelocity()
    {
        return _rigidBody.velocity;
    }

    public void SetGravityScale(float gravityScale)
    {
        _rigidBody.gravityScale = gravityScale;
    }

    public void ResetGravityScale()
    {
        _rigidBody.gravityScale = gravity;
    }

    public void Move(Vector2 direction)
    {
        _nextDirection = direction;
    }

    public void Jump(float jumpPower)
    {
        float cancelForce = _rigidBody.velocity.y * (-1) * _rigidBody.mass;
        _rigidBody.AddForce(Vector2.up * (cancelForce + jumpPower), ForceMode2D.Impulse);
    }
    
    public void Jump(float jumpPower, Vector2 direction)
    {
        float cancelForce = _rigidBody.velocity.y * (-1) * _rigidBody.mass;
        _rigidBody.AddForce(direction * (cancelForce + jumpPower), ForceMode2D.Impulse);
    }

    public void ClimbState()
    {
        _nextDirection = Vector2.zero;
        _rigidBody.velocity = Vector2.zero;
        SetGravityScale(0);
    }
    
    public void EndClimbState()
    {
        ResetGravityScale();
    }

    /// <summary>
    /// 넉백 효과를 적용하는 함수
    /// </summary>
    /// <param name="knockbackDirection">넉백될 방향 (정규화된 벡터)</param>
    /// <param name="knockbackDistance">넉백 거리 (기본값: 6)</param>
    /// <param name="knockbackDuration">넉백 지속 시간 (기본값: 0.3초)</param>
    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackDistance = 6f, float knockbackDuration = 0.3f)
    {
        if (!_isKnockedBack)
        {
            StartCoroutine(KnockbackCoroutine(knockbackDirection.normalized, knockbackDistance, knockbackDuration));
        }
    }
    
    /// <summary>
    /// AddForce를 사용한 넉백 효과를 처리하는 코루틴
    /// </summary>
    private IEnumerator KnockbackCoroutine(Vector2 direction, float distance, float duration)
    {
        Debug.Log("Knockback");
        _isKnockedBack = true;
    
        // 현재 velocity 초기화
        _rigidBody.velocity = Vector2.zero;
    
        // 초기 강한 힘 계산 (전체 거리를 duration 동안 이동하도록)
        float initialForce = (distance * _rigidBody.mass) / (duration * 0.5f);
    
        // 초기 강한 힘 적용
        _rigidBody.AddForce(direction.normalized * initialForce, ForceMode2D.Impulse);
    
        float elapsedTime = 0f;
        Vector2 initialVelocity = _rigidBody.velocity;
    
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float progress = elapsedTime / duration;
        
            // Ease-out 효과를 위한 감속 계산 (cubic ease-out)
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            float velocityMultiplier = 1f - easeProgress;
        
            // 현재 속도를 점진적으로 감소시킴
            Vector2 targetVelocity = initialVelocity * velocityMultiplier;
            _rigidBody.velocity = targetVelocity;
        
            yield return new WaitForFixedUpdate();
        }
    
        // 넉백 종료 시 velocity 초기화
        _rigidBody.velocity = Vector2.zero;
        _isKnockedBack = false;
    }
    
    /// <summary>
    /// 현재 넉백 상태인지 확인하는 함수
    /// </summary>
    /// <returns>true: 넉백 중, false: 정상 상태</returns>
    public bool IsKnockedBack()
    {
        return _isKnockedBack;
    }
    
    /// <summary>
    /// 오브젝트가 땅(Platform)에 있는지 확인하는 함수
    /// </summary>
    /// <returns>true: 땅에 있음 false: 땅에 있지 않음</returns>
    public bool CheckIsGround()
    {
        Vector2 rayPos = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
        Debug.DrawRay(rayPos, Vector2.down * 2f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rayPos, Vector2.down, 2, groundMask);
        
        if (rayHit.collider != null)
        {
            if (rayHit.distance < 0.1f)
            {
                return true;
            }
        }
        return false;
    }

}
