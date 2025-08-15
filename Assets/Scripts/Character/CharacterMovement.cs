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

    private Vector2 _nextDirection;
    private bool _isGround;
    private bool _isWallClimbing;
    private bool _isRopeClimbing;

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
