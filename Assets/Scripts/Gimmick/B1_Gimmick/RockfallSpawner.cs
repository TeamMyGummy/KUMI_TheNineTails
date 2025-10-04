using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockfallSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private Transform[] spawnPoints = new Transform[3];
    [SerializeField] private GameObject rockPrefab;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float spawnInterval = 3f; // 낙석 간격(전체 주기)

    [Header("Count")]
    [SerializeField, Min(1)] private int totalLayers = 5; // 총 드롭 수 = totalLayers * 3

    [Header("Warning Marker")]
    [SerializeField, Min(0f)] private float preWarnTime = 1f; // 떨어지기 전에 경고가 보이는 시간
    [SerializeField] private GameObject warnMarkerPrefab;     // 빨간 타원 프리팹
    [SerializeField] private LayerMask groundMask;            // 바닥 레이어
    [SerializeField, Min(0.1f)] private float raycastDistance = 50f;
    [SerializeField] private Vector3 warnMarkerOffset = new Vector3(0, 0.05f, 0);

    [Header("Start")]
    [SerializeField] private bool autoStartOnPlay = true;

    private Coroutine _spawnRoutine;
    private readonly int[] _heights = new int[3]; // 각 열(1,2,3)의 현재 높이(논리 카운트)

    private void Start()
    {
        if (autoStartOnPlay) StartSpawning();
    }

    public void StartSpawning()
    {
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        System.Array.Clear(_heights, 0, _heights.Length);
        _spawnRoutine = StartCoroutine(SpawnRocks());
    }

    private IEnumerator SpawnRocks()
    {
        if (rockPrefab == null || spawnPoints.Length != 3 ||
            spawnPoints[0] == null || spawnPoints[1] == null || spawnPoints[2] == null)
        {
            Debug.LogError("[RockfallSpawner] 설정이 올바르지 않습니다.");
            yield break;
        }

        float postSpawnWait = Mathf.Max(0f, spawnInterval - preWarnTime);
        int totalDrops = totalLayers * 3;

        for (int k = 0; k < totalDrops; k++)
        {
            // 1) 브리지(bridge) 규칙에 맞는 유효 열 찾기
            List<int> valid = GetBridgeableColumns(_heights);

            int chosen;
            if (valid.Count > 0)
            {
                chosen = valid[Random.Range(0, valid.Count)];
            }
            else
            {
                chosen = ChooseLeastImbalanceColumn(_heights);
            }

            Transform dropPoint = spawnPoints[chosen];

            // 2) 경고 마커 생성
            Vector3 groundPos = GetGroundPoint(dropPoint.position);
            GameObject marker = null;
            if (warnMarkerPrefab != null)
            {
                marker = Instantiate(warnMarkerPrefab, groundPos + warnMarkerOffset, Quaternion.identity);
            }

            // 3) 경고 유지
            if (preWarnTime > 0f)
                yield return new WaitForSeconds(preWarnTime);

            // 4) 낙석 생성
            Instantiate(rockPrefab, dropPoint.position, Quaternion.identity);

            // 5) 높이 갱신
            _heights[chosen]++;

            // 6) 마커 제거
            if (marker != null) Destroy(marker);

            // 7) 다음 낙석까지 대기
            if (k < totalDrops - 1 && postSpawnWait > 0f)
                yield return new WaitForSeconds(postSpawnWait);
        }

        _spawnRoutine = null;
    }

    /// <summary>
    /// 브리지 규칙 검사:
    /// 정렬된 높이 세 값의 인접 차가 모두 1 이하이면 true
    /// </summary>
    private List<int> GetBridgeableColumns(int[] h)
    {
        var list = new List<int>(3);
        for (int i = 0; i < 3; i++)
        {
            int h0 = h[0], h1 = h[1], h2 = h[2];
            if (i == 0) h0++;
            else if (i == 1) h1++;
            else h2++;

            int a = h0, b = h1, c = h2;
            if (a > b) (a, b) = (b, a);
            if (b > c) (b, c) = (c, b);
            if (a > b) (a, b) = (b, a);

            bool bridgeable = (b - a <= 1) && (c - b <= 1);
            if (bridgeable) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// 브리지 조건을 만족하는 열이 없을 때,
    /// 인접 차이의 최대값이 최소가 되는 열 선택 (안전장치)
    /// </summary>
    private int ChooseLeastImbalanceColumn(int[] h)
    {
        int bestIdx = 0;
        int bestScore = int.MaxValue;

        for (int i = 0; i < 3; i++)
        {
            int h0 = h[0], h1 = h[1], h2 = h[2];
            if (i == 0) h0++;
            else if (i == 1) h1++;
            else h2++;

            int a = h0, b = h1, c = h2;
            if (a > b) (a, b) = (b, a);
            if (b > c) (b, c) = (c, b);
            if (a > b) (a, b) = (b, a);

            int score = Mathf.Max(b - a, c - b);
            if (score < bestScore)
            {
                bestScore = score;
                bestIdx = i;
            }
        }
        return bestIdx;
    }

    private Vector3 GetGroundPoint(Vector3 from)
    {
        RaycastHit2D hit = Physics2D.Raycast(from, Vector2.down, raycastDistance, groundMask);
        if (hit.collider != null)
            return hit.point;
        return from + Vector3.down * 1.0f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var t in spawnPoints)
        {
            if (t == null) continue;
            Gizmos.DrawSphere(t.position, 0.08f);
        }
    }
#endif
}
