using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MovePattern
{
    Patrol, //기본(왕복 운동)
    Aggro, //어그로 끌렸을 때
    Return //어그로 해제 시 스폰 위치로 돌아감
}
public class MonsterMovement : MonoBehaviour
{
    private CharacterMovement cm;
    private Monster monster;

    private int dir = 1;
    private Vector2 spawnPos; //스폰된 위치
    private Vector2 patrolPos; //왕복 기준점
    private MovePattern moveState = MovePattern.Patrol;

    [SerializeField] private float moveRange = 6; //6칸 왕복 이동 디폴트
    [SerializeField] private LayerMask platformLayer;

    //정지
    private bool isPaused = false;
    private float pauseTimer = 0;
    private float pausedTime = 3;    

    //어그로
    [SerializeField] private Transform player;
    private float aggroRange = 0;
    private float aggroReleaseRange = 14; //14칸 벗어나면 어그로 해제

    void Start()
    {
        cm = GetComponent<CharacterMovement>();
        monster = GetComponent<Monster>();
        spawnPos = transform.position;
        patrolPos = spawnPos + new Vector2(-moveRange / 2f, 0);

        //aggroRange 정보 받아오기
        if (monster.asc.Attribute.Attributes.TryGetValue("AggroRange", out var attr))
        {
            aggroRange = attr.CurrentValue.Value;
        }
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        
        //aggroRange 안에 들어오면 어그로 끌림
        if (dist <= aggroRange)
            moveState = MovePattern.Aggro;
        //14칸 이상 멀어지면 어그로 해제
        else if (moveState == MovePattern.Aggro && dist >= aggroReleaseRange)
            moveState = MovePattern.Return;
        
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
                    patrolPos = spawnPos + new Vector2(-moveRange / 2f, 0);
                }
                break;
            }

    }
    private void PatrolMove()
    {
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pausedTime)
            {
                isPaused = false;
                pauseTimer = 0;
                dir *= -1;
                patrolPos = transform.position;
            }
            cm.Move(Vector2.zero);
            return;
        }

        if (!CheckGroundAhead())
        {
            isPaused = true;
            cm.Move(Vector2.zero);
            return;
        }

        cm.Move(Vector2.right * dir);

        float movedDistance = Mathf.Abs(transform.position.x - patrolPos.x);
        if (movedDistance >= moveRange)
        {
            dir *= -1;
            patrolPos = transform.position;
        }
    }


    private void AggroMove()
    {
        dir = player.position.x > transform.position.x ? 1 : -1;

        if (!CheckGroundAhead())
        {
            
            cm.Move(Vector2.zero);
            return;
        }
        cm.Move(Vector2.right * dir);
    }

   
    private void ReturnMove()
    {
        //스폰 지점으로 돌아가면 Patrol로 전환
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


    //앞에 땅이 있나 체크
    private bool CheckGroundAhead()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(dir * 0.8f, -0.2f);
        float checkDistance = 1.2f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDistance, platformLayer);
        Debug.DrawRay(checkPos, Vector2.down * checkDistance, Color.red);
        return hit.collider != null;
    }


}