using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rigid;

    [Range(0.0f, 3.0f)] public float speed;
    [Range(0.0f, 7.0f)] public float jumpPower;
    [Range(0.0f, 3.0f)] public float gravity;

    private Vector2 nextDirection;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = gravity;
    }

    private void FixedUpdate()
    {
        if(nextDirection != Vector2.zero)
        {
            Vector2 currMove = rigid.position;
            Vector2 nextMove = nextDirection * speed;

            rigid.position = currMove + nextMove;
            //rigid.velocity = nextMove / Time.deltaTime;
        }
        
        if(rigid.velocity.y < 0.0f)
        {
            //Debug.DrawRay(rigid.position, Vector2.down, new Color(1, 0, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1, LayerMask.GetMask("Platform"));
            if(rayHit.collider != null)
            {
                if(rayHit.distance < 1.0f)
                {
                    GetComponent<PlayerController>().OnEnableJump();
                }
            }
        }
    }

    public void Move(Vector2 direction)
    {
        nextDirection = direction;
    }

    public void Jump()
    {
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
}
