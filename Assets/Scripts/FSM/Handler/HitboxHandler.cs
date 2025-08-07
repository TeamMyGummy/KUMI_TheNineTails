using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitboxHandler : ActionHandler
{
    private Collider2D _collider2D;

    private Vector2 _size;
    private Vector2 _offset;
    private float _timer;
    private float _elapsed;

    void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _collider2D.enabled = false; // 초기에는 꺼두기
    }

    public void SetHitbox(Vector2 size, Vector2 offset, float timer)
    {
        _size = size;
        _offset = offset;
        _timer = timer;
    }

    public override void OnEnterAction()
    {
        _elapsed = 0f;
        ApplyHitbox();
        _collider2D.enabled = true;
    }

    public override bool OnExecuteAction()
    {
        _elapsed += Time.deltaTime;

        if (_elapsed >= _timer)
        {
            _collider2D.enabled = false;
            return true; // Action 완료
        }

        return false; // 아직 진행 중
    }

    private void ApplyHitbox()
    {
        // 부모 좌표 기준 오프셋 적용
        // BoxCollider2D와 CircleCollider2D 지원 (필요시 확장 가능)
        if (_collider2D is BoxCollider2D box)
        {
            box.size = _size;
            box.offset = _offset;
        }
        else if (_collider2D is CircleCollider2D circle)
        {
            // size.x를 지름으로 가정
            circle.radius = _size.x * 0.5f;
            circle.offset = _offset;
        }
        else
        {
            Debug.LogWarning($"{name}: HitboxHandler에서 지원하지 않는 Collider2D 타입입니다.");
        }
    }
}