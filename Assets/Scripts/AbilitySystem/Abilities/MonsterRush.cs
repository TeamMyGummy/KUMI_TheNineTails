using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MonsterRush : BlockAbility<MonsterRushSO>
{
    private MonsterMovement _movement;   // 공통 이동 컴포넌트 (지상/비행 모두)
    private Monster _monster;
    private MonsterRushSO _rushData;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        CanReuse = true;

        _movement = actor.GetComponent<MonsterMovement>();
        _monster  = actor.GetComponent<Monster>();
        _rushData = abilitySo as MonsterRushSO;
    }

    protected override bool CanActivate()
    {
        if (Asc.TagContainer.Has(GameplayTags.BlockRunningAbility))
            return false;
        
        if (!_monster.Data.IsFlying && _movement != null && !_movement.CheckGroundAhead())
            return false;
        
        return true;
    }

    protected override void Activate()
    {
        base.Activate();
        Rush().Forget();
    }

    private async UniTaskVoid Rush()
    {
        if (_monster == null || _rushData == null) return;

        if (_movement != null)
        {
            _movement.LockDirection(true);
            _movement.SetPaused(true);
            _movement.GetComponent<CharacterMovement>()?.Move(Vector2.zero);
        }

        _monster.StartCoroutine(_monster.Flash());
        await UniTask.Delay(1000);

        Vector2 startPos = _monster.transform.position;

        Transform playerTr =
            (_monster != null && _monster.Player != null)
            ? _monster.Player
            : GameObject.FindWithTag("Player")?.transform;

        Vector2 playerPos = playerTr != null ? (Vector2)playerTr.position : startPos;

        Vector2 rushDir;
        Vector2 targetPos;

        if (_monster.Data.IsFlying)
        {
            rushDir = (playerPos - startPos).normalized;
            if (rushDir == Vector2.zero)
                rushDir = Vector2.right * (_movement != null ? Mathf.Sign(_movement.HorizontalDir) : 1f);

            targetPos = startPos + rushDir * _rushData.RushDistance;
        }
        else
        {
            float dirX = (_movement != null) ? Mathf.Sign(_movement.HorizontalDir) : 1f;
            rushDir = new Vector2(dirX, 0f);
            targetPos = startPos + rushDir * _rushData.RushDistance;
        }

        GameObject hitbox = SpawnHitbox(rushDir);
        float rushSpeed = 20f;

        while (Vector2.Distance(_monster.transform.position, targetPos) > 0.05f)
        {
            //지상몹은 땅이 없으면 멈추게
            if (!_monster.Data.IsFlying && _movement != null && !_movement.CheckGroundAhead())
                break;

            Vector2 nextPos = Vector2.MoveTowards(
                _monster.transform.position, targetPos, rushSpeed * Time.deltaTime);

            if (_monster.Data.IsFlying)
            {
                //날몹은 땅에 닿으면 멈추게
                RaycastHit2D groundHit = Physics2D.Raycast(
                    _monster.transform.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));
                if (groundHit.collider != null && nextPos.y <= groundHit.point.y + 0.1f)
                {
                    nextPos.y = groundHit.point.y + 0.1f;
                    _monster.transform.position = nextPos;
                    break;
                }
            }

            _monster.transform.position = nextPos;
            await UniTask.Yield();
        }

        if (hitbox != null) GameObject.Destroy(hitbox);

        if (_movement != null)
        {
            _movement.SetPaused(false);
            _movement.LockDirection(false);
        }

        AbilityFactory.Instance.EndAbility(this);
    }

    private GameObject SpawnHitbox(Vector2 direction)
    {
        float attackRangeX = _rushData.AttackRangeX;
        float attackRangeY = _rushData.AttackRangeY;

        GameObject hitbox = ResourcesManager.Instance.Instantiate(_rushData.AttackHitboxPrefab);
        hitbox.transform.SetParent(_monster.transform);
        hitbox.transform.localPosition = new Vector2(direction.x * 1f, direction.y * 0.2f);

        var box = hitbox.GetComponent<BoxCollider2D>();
        if (box != null)
            box.size = new Vector2(attackRangeX, attackRangeY);

        var sr = hitbox.GetComponent<SpriteRenderer>();
        if (sr != null && sr.drawMode != SpriteDrawMode.Simple)
            sr.size = new Vector2(attackRangeX, attackRangeY);

        return hitbox;
    }
}
