using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    [SerializeField]
    [Range(0.0f, 3.0f)] private float speed;

    [SerializeField]
    [Range(0.0f, 3.0f)] private float gravity;

    private Vector2 _nextDirection;
    private bool isGround;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = gravity;
    }

    private void FixedUpdate()
    {
        //  Move
        if(_nextDirection != Vector2.zero)
        {
            Vector2 currMove = _rigidBody.position;
            Vector2 nextMove = _nextDirection * speed;

            _rigidBody.position = currMove + nextMove;
            //rigid.velocity = nextMove / Time.deltaTime;
        }

        // Jump
        isGround = CheckIsGround();

    }

    public Vector2 GetCharacterDirection()
    {
        return _nextDirection;
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
            if (rayHit.distance < 0.6f)
            {
                return true;
            }
        }
        return false;
    }

}
