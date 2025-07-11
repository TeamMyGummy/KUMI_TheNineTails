using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovePattern
{
    Monster1,
    Monster2,

}
public class MonsterMovement : MonoBehaviour
{
    private CharacterMovement cm;
    private float timer = 0f;
    private int dir = 1;

    [SerializeField] private MovePattern movePattern;


    void Start()
    {
        cm = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (movePattern)
        {
            case MovePattern.Monster1:
                
                Monster1Move();
                break;

            case MovePattern.Monster2:
                Monster2Move();
                break;

        }
    }

    private void Monster1Move()//ÁÂ¿ì ¿Õº¹
    {
        float switchInterval = 1f;
        if (timer >= switchInterval)
        {
            timer = 0f;
            dir *= -1;
        }

        cm.Move(Vector2.right * dir);
    }

    private void Monster2Move()
        //ÁÂ¿ì·Î ¿Õº¹ÇÏ¸é¼­ ¶Ù°Ô ÇØ”f½À´Ï´Ù
    {
        Monster1Move();
        if (cm.CheckIsGround())
        {
            cm.Jump(5);
        }
    }
}

