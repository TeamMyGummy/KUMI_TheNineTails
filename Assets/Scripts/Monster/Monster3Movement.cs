using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster3Movement : MonoBehaviour
{
    private CharacterMovement cm;
    private Monster monster;
    private Collider2D monsterCollider;

    private int dir = 1;
    private Vector2 directionToPlayer = Vector2.zero; //추가
    private Vector2 spawnPos;
    private Vector2 patrolPos;
    private MovePattern moveState = MovePattern.Patrol;

    private bool isPaused = false;
    private float pauseTimer = 0;
    private Transform player;

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform headPivot;
    
    private bool canChangeDirection = true;
    private float directionCheckTimer = 0f;
    private readonly float directionCheckDelay = 0.2f;
    
    
    private void Start()
    {
        cm = GetComponent<CharacterMovement>();
        monster = GetComponent<Monster>();
        monsterCollider = GetComponent<Collider2D>();

        spawnPos = transform.position;
        patrolPos = spawnPos + new Vector2(-monster.Data.PatrolRange / 2f, 0);

        player = monster.Player;

        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (monster.isAggro)
        {
            moveState = MovePattern.Aggro;
        }
        else if (moveState == MovePattern.Aggro && dist >= monster.Data.AggroReleaseRange)
        {
            moveState = MovePattern.Return;
        }

        switch (moveState)
        {
            case MovePattern.Patrol:
                PatrolMove();
                break;
            case MovePattern.Aggro:
                AggroMove();
                break;
            case MovePattern.Return:
                ReturnMove();
                if (Mathf.Abs(transform.position.x - spawnPos.x) < 0.1f)
                {
                    moveState = MovePattern.Patrol;
                    dir = 1;
                    patrolPos = spawnPos + new Vector2(-monster.Data.PatrolRange / 2f, 0);
                }

                break;
        }
    }
    
    public void ChangeMovePattern(MovePattern pattern)
    {
        moveState = pattern;
    }
    
    private void PatrolMove()
    {
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= monster.Data.PausedTime)
            {
                isPaused = false;
                pauseTimer = 0;
                dir *= -1;
                patrolPos = transform.position;
            }

            cm.Move(Vector2.zero);
            return;
        }

        /*if (!monster.Data.IsFlying && !CheckGroundAhead())
        {
            isPaused = true;
            cm.Move(Vector2.zero);
            return;
        }*/

        cm.Move(Vector2.right * dir);

        float movedDistance = Mathf.Abs(transform.position.x - patrolPos.x);
        if (movedDistance >= monster.Data.PatrolRange)
        {
            dir *= -1;
            patrolPos = transform.position;
        }
    }
    
    private void AggroMove()
    {
        if (canChangeDirection)
        {
            directionCheckTimer -= Time.deltaTime;
            if (directionCheckTimer <= 0f)
            {
                directionCheckTimer = directionCheckDelay;
                directionToPlayer = ((Vector2)headPivot.position - (Vector2)transform.position).normalized;
            }
        }
        
        /*if (!monster.Data.IsFlying && !CheckGroundAhead())
        {
            cm.Move(Vector2.zero);
            return;
        }*/
        
        /*cm.Move(Vector2.right * dir);*/
        cm.Move(directionToPlayer);
    }


    private void ReturnMove()
    {
        float dist = Vector2.Distance(transform.position, spawnPos);

        if (dist <= 0.1f)
        {
            isPaused = false;
            patrolPos = transform.position;
            dir = 1;
            moveState = MovePattern.Patrol;
            return;
        }

        dir = spawnPos.x > transform.position.x ? 1 : -1;
        cm.Move(Vector2.right * dir);
    }

    /*public bool CheckGroundAhead()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(dir * 0.8f, -0.2f);
        float checkDistance = 1.2f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDistance, platformLayer);
        return hit.collider != null;
    }*/

    /*public int GetDirection() => dir;*/
    public Vector2 GetDirection() => directionToPlayer;
    
    public void LockDirection(bool lockDir)
    {
        canChangeDirection = !lockDir;
    }
}
