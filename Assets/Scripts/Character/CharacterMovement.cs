using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _sprite;

    [SerializeField] [Range(0.0f, 3.0f)] private float speed;

    [SerializeField] [Range(0.0f, 3.0f)] private float gravity;
    public float Gravity => gravity;

    private Vector2 _nextDirection;
    private bool _isGround;
    private bool _isWallClimbing;
    private bool _isRopeClimbing;
    private bool _isKnockedBack;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _rigidBody.gravityScale = gravity;
    }

    private void FixedUpdate()
    {
        //  Move
        if (_nextDirection != Vector2.zero)
        {
            Vector2 currMove = _rigidBody.position;
            Vector2 nextMove = _nextDirection * speed;

            _rigidBody.position = currMove + nextMove;
            //rigid.velocity = nextMove / Time.deltaTime;
        }

        // Jump
        _isGround = CheckIsGround();
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
        _rigidBody.gravityScale = 0;
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
    /// 넉백 효과를 처리하는 코루틴
    /// </summary>
    private IEnumerator KnockbackCoroutine(Vector2 direction, float distance, float duration)
    {
        _isKnockedBack = true;
        
        // 현재 velocity 초기화
        _rigidBody.velocity = Vector2.zero;
        
        Vector2 startPosition = _rigidBody.position;
        Vector2 targetPosition = startPosition + (direction * distance);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float progress = elapsedTime / duration;
            
            // Ease-out 효과 적용 (처음엔 빠르게, 나중엔 천천히)
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, easeProgress);
            _rigidBody.position = currentPosition;
            
            yield return new WaitForFixedUpdate();
        }
        
        // 최종 위치 설정
        _rigidBody.position = targetPosition;
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
    
    public void StartWallClimbState()
    {
        // 벽타기 상태
        ClimbState();
        _isWallClimbing = true;
    }

    public void EndWallClimbState()
    {
        _rigidBody.gravityScale = gravity;
        _isWallClimbing = false;
    }

    public void StartRopeClimbState()
    {
        // 밧줄 타기 상태
        ClimbState();
        _isRopeClimbing = true;
    }

    public void EndRopeClimbState()
    {
        _rigidBody.gravityScale = gravity;
        _isRopeClimbing = false;
    }

    public bool CheckIsWallClimbing()
    {
        return _isWallClimbing;
    }

    public bool CheckIsRopeClimbing()
    {
        return _isRopeClimbing;
    }

    public bool CheckIsClimbing()
    {
        return _isWallClimbing || _isRopeClimbing;
    }
    
    /// <summary>
    /// 오브젝트가 땅(Platform)에 있는지 확인하는 함수
    /// </summary>
    /// <returns>true: 땅에 있음 false: 땅에 있지 않음</returns>
    public bool CheckIsGround()
    {
        Debug.DrawRay(_rigidBody.position, Vector2.down, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(_rigidBody.position, Vector2.down, 10, LayerMask.GetMask("Platform"));
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
