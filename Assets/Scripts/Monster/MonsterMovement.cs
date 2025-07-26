using System.Collections;
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
    private Collider2D monsterCollider;

    private int dir = 1;
    private Vector2 spawnPos;
    private Vector2 patrolPos;
    private MovePattern moveState = MovePattern.Patrol;

    private bool isPaused = false;
    private float pauseTimer = 0;

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform player;

    private void Start()
    {
        cm = GetComponent<CharacterMovement>();
        monster = GetComponent<Monster>();
        monsterCollider = GetComponent<Collider2D>();

        spawnPos = transform.position;
        patrolPos = spawnPos + new Vector2(-monster.Data.PatrolRange / 2f, 0);

        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // 시야각 계산
        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 forward = Vector2.right * dir;
        float angle = Vector2.Angle(forward, toPlayer);

        if (dist <= monster.Data.AggroRange && angle <= monster.Data.ViewSight / 2f)
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

        if (!monster.Data.IsFlying && !CheckGroundAhead())
        {
            isPaused = true;
            cm.Move(Vector2.zero);
            return;
        }

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
        dir = player.position.x > transform.position.x ? 1 : -1;

        if (!monster.Data.IsFlying && !CheckGroundAhead())
        {
            cm.Move(Vector2.zero);
            return;
        }

        if (IsPlayerTouched())
        {
            cm.Move(Vector2.zero); // 플레이어랑 부딪혔으면 멈춤
        }
        else
        {
            cm.Move(Vector2.right * dir);
        }

        monster.asc.TryActivateAbility(AbilityKey.MonsterAttack);
    }


    private bool IsPlayerTouched()
    {
        if (player == null || monsterCollider == null) return false;

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null) return false;

        return monsterCollider.IsTouching(playerCollider);
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

    private bool CheckGroundAhead()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(dir * 0.8f, -0.2f);
        float checkDistance = 1.2f;

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDistance, platformLayer);
        return hit.collider != null;
    }

    public int GetDirection() => dir;

    
    //어그로 범위, 공격 범위 시각화
    private void OnDrawGizmosSelected()
    {
    #if UNITY_EDITOR
        if (monster == null) monster = GetComponent<Monster>();
        if (monster == null || monster.Data == null) return;

        Vector3 origin = transform.position;
        Vector2 forward = Vector2.right * dir;

        // 1. 시야각 + 어그로 범위
        float viewSight = monster.Data.ViewSight;
        float aggroRange = monster.Data.AggroRange;
        float releaseRange = monster.Data.AggroReleaseRange;

        float halfAngle = viewSight / 2f;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // 노랑
        Vector3 left = Quaternion.Euler(0, 0, -halfAngle) * forward;
        Vector3 right = Quaternion.Euler(0, 0, halfAngle) * forward;
        Gizmos.DrawRay(origin, left * aggroRange);
        Gizmos.DrawRay(origin, right * aggroRange);

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f); // 초록
        Gizmos.DrawWireSphere(origin, aggroRange);

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // 빨강
        Gizmos.DrawWireSphere(origin, releaseRange);

        // 2. 공격 범위 표시
        float rangeX = monster.Data.AttackRangeX;
        float rangeY = monster.Data.AttackRangeY;
        int dirCode = monster.Data.AttackDir;

        float angleDeg = 0f;
        switch (dirCode)
        {
            case 1: angleDeg = 0f; break;
            case 2: angleDeg = 270f; break;
        }

        if ((dirCode == 1 || dirCode == 4) && dir == -1)
        {
            angleDeg = 180f - angleDeg;
        }

        float angleRad = angleDeg * Mathf.Deg2Rad;
        Vector2 attackDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
        Vector2 offset = attackDir * new Vector2(rangeX / 2f, rangeY / 2f);
        Vector2 attackOrigin = (Vector2)transform.position + offset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackOrigin, new Vector2(rangeX, rangeY));
#endif
    }
}
