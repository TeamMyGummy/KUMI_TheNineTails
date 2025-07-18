using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovePattern
{
    Patrol,
    Aggro,
    Return
}

public class MonsterMovement : MonoBehaviour
{
    private CharacterMovement cm;
    private Monster monster;

    private int dir = 1;
    private Vector2 spawnPos;
    private Vector2 patrolPos;
    private MovePattern moveState = MovePattern.Patrol;

    [SerializeField] private float moveRange = 6f;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform player;

    private bool isPaused = false;
    private float pauseTimer = 0f;
    private float pausedTime = 3f;
    
    private float aggroReleaseRange = 14f; //어그로 해제 거리
    private float aggroRange = 8f; //부채꼴의 반지름
    private float sightAngle = 22.5f; // 45도 부채꼴
    private float attackRangeX = 0; //공격 범위 가로 칸수
    private float attackRangeY = 0; //공격 범위 세로 칸수

    void Start()
    {
        cm = GetComponent<CharacterMovement>();
        monster = GetComponent<Monster>();
        spawnPos = transform.position;
        patrolPos = spawnPos + new Vector2(-moveRange / 2f, 0);

        var attr = monster.asc.Attribute.Attributes;

        aggroRange   = attr["AggroRange"].CurrentValue.Value;
        attackRangeX  = attr["AttackRangeX"].CurrentValue.Value;
        attackRangeY  = attr["AttackRangeY"].CurrentValue.Value;
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (IsPlayerInSight())
        {
            moveState = MovePattern.Aggro;
        }
        else if (moveState == MovePattern.Aggro && dist >= aggroReleaseRange)
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
                    patrolPos = spawnPos + new Vector2(-moveRange / 2f, 0);
                }
                break;
        }
    }

    public int GetDirection()
    {
        return dir;
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

    //어그로 끌렸을 때
    private void AggroMove()
    {
        dir = player.position.x > transform.position.x ? 1 : -1;

        if (!CheckGroundAhead())
        {
            cm.Move(Vector2.zero);
            return;
        }

        cm.Move(Vector2.right * dir);

        if (IsPlayerInSight())
        {
            Vector2 offset = player.position - transform.position;
            //실제 사거리 0.75배 범위 안에 들어와야 공격 시작
            if (Mathf.Abs(offset.x) <= attackRangeX * 0.75 && Mathf.Abs(offset.y) <= attackRangeY * 0.75)
            {
                Debug.Log("[MonsterMovement] 시야 + 범위 충족 → 어빌리티 발동");
                monster.asc.TryActivateAbility(AbilityKey.MonsterAttack);
            }
        }
    }

    //어그로 해제 시 스폰위치로 이동
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

    //전방 구덩이인지 체크
    private bool CheckGroundAhead()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(dir * 0.8f, -0.2f);
        float checkDistance = 1.2f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDistance, platformLayer);
        Debug.DrawRay(checkPos, Vector2.down * checkDistance, Color.red);
        return hit.collider != null;
    }

    //플레이어가 몬스터 시야 범위에 들어왔는지 체크
    private bool IsPlayerInSight()
    {
        Vector2 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > aggroRange) return false;

        Vector2 forward = new Vector2(GetDirection(), 0).normalized;
        float angle = Vector2.Angle(forward, toPlayer.normalized);

        return angle <= sightAngle;
    }


    //몬스터 시야 / 공격 범위 확인 위한 디버깅용
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        int dir = GetDirection();
        Vector3 forward = Vector3.right * dir;

        // 시야 시각화
        Quaternion rot1 = Quaternion.Euler(0, 0, sightAngle * -dir);
        Quaternion rot2 = Quaternion.Euler(0, 0, -sightAngle * -dir);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, rot1 * forward * aggroRange);
        Gizmos.DrawRay(origin, rot2 * forward * aggroRange);
        Gizmos.DrawWireSphere(origin, aggroRange);

        // 공격 범위 시각화
        if (monster == null || monster.asc == null) return;

        var attr = monster.asc.Attribute.Attributes;

        if (!attr.ContainsKey("AttackRangeX") || !attr.ContainsKey("AttackRangeY")) return;

        float attackRangeX = attr["AttackRangeX"].CurrentValue.Value;
        float attackRangeY = attr["AttackRangeY"].CurrentValue.Value;

        Vector2 size = new Vector2(attackRangeX, attackRangeY);
        Vector2 center = (Vector2)origin + new Vector2(dir * size.x / 2f, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }

    
    
#endif

}
