using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rigid;

    [SerializeField]
    [Range(0.0f, 3.0f)] private float speed;

    [SerializeField]
    [Range(0.0f, 7.0f)] private float jumpPower;

    [SerializeField]
    [Range(0.0f, 3.0f)] private float gravity;

    private Vector2 nextDirection;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = gravity;
    }

    private void FixedUpdate()
    {
        //  Move
        if(nextDirection != Vector2.zero)
        {
            Vector2 currMove = rigid.position;
            Vector2 nextMove = nextDirection * speed;

            rigid.position = currMove + nextMove;
            //rigid.velocity = nextMove / Time.deltaTime;
        }
        
        // Jump
        if(rigid.velocity.y < 0.0f)
        {
            //Debug.DrawRay(rigid.position, Vector2.down, new Color(1, 0, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 2, LayerMask.GetMask("Platform"));
            if(rayHit.collider != null)
            {
                if(rayHit.distance < 0.5f)
                {
                    if (GetComponent<PlayerController>() != null)
                    {
                        GetComponent<PlayerController>().OnEnableJump();
                    }
                    
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
