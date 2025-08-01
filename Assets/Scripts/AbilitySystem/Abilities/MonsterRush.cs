using GameAbilitySystem;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MonsterRush : BlockAbility
{
    private MonsterMovement _movement;
    private Monster _monster;
    private MonsterRushSO _rushData;

    public override void InitAbility(GameObject actor, AbilitySystem asc, GameplayAbilitySO abilitySo)
    {
        base.InitAbility(actor, asc, abilitySo);
        CanReuse = true;

        _movement = actor.GetComponent<MonsterMovement>();
        _monster = actor.GetComponent<Monster>();
        _rushData = abilitySo as MonsterRushSO;
    }

    protected override bool CanActivate()
    {
        return !Asc.TagContainer.Has(GameplayTags.BlockRunningAbility);
    }

    protected override void Activate()
    {
        base.Activate();
        Rush().Forget();
    }
    private async UniTaskVoid Rush()
    {
        if (_monster == null) return;
        _movement.enabled = false;

        //돌진 전 반짝 효과
        _monster.StartCoroutine(_monster.Flash());
        //임의로 넣은 딜레이
        await UniTask.Delay(1000);
        
        int facingDir = _movement.GetDirection();
        float rushDistance = _rushData.RushDistance;
        float rushSpeed = 20f;
        Vector2 startPos = _monster.transform.position;
        Vector2 targetPos = startPos + new Vector2(facingDir * rushDistance, 0f);

        GameObject hitbox = SpawnHitbox();
        while (Vector2.Distance(_monster.transform.position, targetPos) > 0.05f)
        {
            Vector2 nextPos = Vector2.MoveTowards(_monster.transform.position, targetPos, rushSpeed * Time.deltaTime);
            //돌진 중 앞에 땅 없으면 멈춤
            if (!_movement.CheckGroundAhead())
            {
                break;
            }
            _monster.transform.position = nextPos;
            await UniTask.Yield();
        }
        GameObject.Destroy(hitbox);
        _movement.enabled = true;
        AbilityFactory.Instance.EndAbility(this);
    }

    private GameObject SpawnHitbox()
    {
        int facingDir = _movement.GetDirection();
        Vector2 hitboxPos = new Vector2(facingDir * 1f, 0f);
        float attackRangeX = _rushData.AttackRangeX;
        float attackRangeY = _rushData.AttackRangeY;

        GameObject hitbox = ResourcesManager.Instance.Instantiate(_rushData.AttackHitboxPrefab);
        //히트박스를 자식으로 달아서 몬스터 따라가게...
        hitbox.transform.SetParent(_monster.transform);
        hitbox.transform.localPosition = hitboxPos;

        var box = hitbox.GetComponent<BoxCollider2D>();
        if (box != null)
            box.size = new Vector2(attackRangeX, attackRangeY);
        
        var sr = hitbox.GetComponent<SpriteRenderer>();
        if (sr != null && sr.drawMode != SpriteDrawMode.Simple)
            sr.size = new Vector2(attackRangeX, attackRangeY);
        
        return hitbox;
    }




}
