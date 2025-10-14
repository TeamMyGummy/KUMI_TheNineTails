using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Util; // SceneLoader, StringToInt 등

/// <summary>
/// 플레이어 공격 n회로 부서지는 혼불 상자
/// - BreakableWall 처럼 키를 만들고 파괴 상태 저장
/// - 파괴 시 Honbul 프리팹을 여러개 Instantiate (Monster.Die 방식 참고)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HonbulBox : MonoBehaviour
{
    [Header("파괴 설정")]
    [SerializeField] private int maxHit = 3;               // 몇 번 맞으면 파괴될지
    private int _hitCount = 0;

    [Header("드랍(혼불) 설정")]
    [SerializeField] private GameObject honbulPrefab;     // 기존 Honbul 프리팹 할당
    [SerializeField] private int dropCount = 5;           // 떨어뜨릴 혼불 개수
    [SerializeField] private float spacing = 0.5f;        // spawn 간격 (Monster.Die 유사)
    [SerializeField] private float randomOffset = 0.25f;  // 약간의 랜덤 포지션 보정

    [FormerlySerializedAs("saveBrokenState")]
    [Header("저장 옵션")]
    [SerializeField] private bool isSaveBrokenState = true; // 파괴 상태 저장 여부

    private int _boxKey;
    private bool _isBroken = false;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;

        // 유니크 키 생성 (BreakableWall과 동일 로직)
        _boxKey = SceneLoader.GetCurrentSceneName().StringToInt() + transform.GetSiblingIndex();

        // 이미 부서진 상자라면 제거
        if (isSaveBrokenState)
        {
            // 주의: HonbulBoxState가 GameData에 있어야 합니다 (아래에 설명)
            if (DomainFactory.Instance.Data.HonbulBoxState.BrokenHonbulBoxes.Contains(_boxKey))
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 공격이 닿을 때 외부에서 호출 (AttackRange에서 콜)
    /// </summary>
    public void AttackCount()
    {
        if (_isBroken) return;

        _hitCount++;
        if (_hitCount >= maxHit)
        {
            Broken();
        }
    }

    private void Broken()
    {
        if (_isBroken) return;
        _isBroken = true;

        // 저장
        if (isSaveBrokenState)
        {
            DomainFactory.Instance.Data.HonbulBoxState.BrokenHonbulBoxes.Add(_boxKey);
            DomainFactory.Instance.SaveGameData();
        }

        // 드랍 연출(즉시 스폰)
        SpawnHonbuls();

        // 파괴 이펙트 / 사운드 재생을 여기에 추가 가능

        Destroy(gameObject);
    }

    private void SpawnHonbuls()
    {
        if (honbulPrefab == null || dropCount <= 0) return;

        float startX = -(dropCount - 1) * spacing * 0.5f;
        for (int i = 0; i < dropCount; i++)
        {
            Vector3 basePos = transform.position + new Vector3(startX + i * spacing, 0f, 0f);

            // 약간의 랜덤 보정
            Vector2 rand = Random.insideUnitCircle * randomOffset;
            Vector3 spawnPos = basePos + new Vector3(rand.x, rand.y, 0f);

            var go = Instantiate(honbulPrefab, spawnPos, Quaternion.identity);

            // 초기 물리적 임펄스가 있으면 자연스럽게 퍼지게
            if (go.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 impulse = Random.insideUnitCircle.normalized * Random.Range(0.5f, 2f);
                rb.AddForce(impulse, ForceMode2D.Impulse);
            }
        }
    }
    
}
