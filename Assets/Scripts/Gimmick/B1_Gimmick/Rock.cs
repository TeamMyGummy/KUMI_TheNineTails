using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Rock : MonoBehaviour
{
    [Header("Landing Detection")]
    [SerializeField] private LayerMask landingMask;   // Ground | LockedRock
    [SerializeField, Min(0f)] private float settleSpeed = 0.2f; // 이 이하 속도로 안정화 판단
    [SerializeField, Min(0f)] private float settleTime = 0.15f; // 이 시간 동안 계속 느리면 잠금

    [Header("Snap (Optional)")]
    [SerializeField] private bool snapToGrid = true;
    [SerializeField, Min(0.01f)] private float gridSize = 0.5f; // 그리드 스냅 간격

    private Rigidbody2D _rb;
    private bool _locked;
    private float _belowSpeedTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // 덜컹 방지: 회전 고정 권장 (프리팹에서 해도 됨)
        _rb.freezeRotation = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (_locked) return;

        // "착지 후보"는 landingMask에 포함된 충돌만 고려
        if (((1 << col.collider.gameObject.layer) & landingMask) == 0) return;

        // 충분히 느리면 타이머 진행, 아니면 리셋
        if (_rb.velocity.sqrMagnitude <= settleSpeed * settleSpeed)
        {
            _belowSpeedTimer += Time.fixedDeltaTime;
            if (_belowSpeedTimer >= settleTime)
            {
                LockInPlace();
            }
        }
        else
        {
            _belowSpeedTimer = 0f;
        }
    }

    private void LockInPlace()
    {
        if (_locked) return;
        _locked = true;

        if (snapToGrid)
        {
            Vector3 p = transform.position;
            p.x = Mathf.Round(p.x / gridSize) * gridSize;
            p.y = Mathf.Round(p.y / gridSize) * gridSize;
            transform.position = p;
        }

        // 완전 고정: Static으로 전환 → 플레이어/다른 물체가 밀 수 없음
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _rb.bodyType = RigidbodyType2D.Static;

        // 잠긴 낙석 레이어로 변경하면 다음 낙석이 이걸 바닥처럼 취급해 안정적으로 쌓임
        // (프로젝트에 'LockedRock' 레이어를 만들었다면 주석 해제)
        // gameObject.layer = LayerMask.NameToLayer("LockedRock");
    }
}