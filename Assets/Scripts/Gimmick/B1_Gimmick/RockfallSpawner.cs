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
    [SerializeField, Min(1)] private int totalLayers = 5; // 각 열의 최종 높이(= 층 수)

    [Header("Warning Marker")]
    [SerializeField, Min(0f)] private float preWarnTime = 1f; // 떨어지기 전에 경고가 보이는 시간
    [SerializeField] private GameObject warnMarkerPrefab;     // 빨간 타원 프리팹
    [SerializeField] private LayerMask groundMask;            // 바닥 레이어
    [SerializeField, Min(0.1f)] private float raycastDistance = 50f;
    [SerializeField] private Vector3 warnMarkerOffset = new Vector3(0, 0.05f, 0);

    [Header("Start")]
    [SerializeField] private bool autoStartOnPlay = true;

    private Coroutine _spawnRoutine;
    private readonly int[] _heights = new int[3]; // 각 열(0,1,2)의 현재 높이

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
        int totalDrops = totalLayers * 3; // 최종적으로 각 열이 totalLayers가 되도록

        for (int k = 0; k < totalDrops; k++)
        {
            // SOFT: cap + 인접쌍(|h0-h1|, |h1-h2|) 제약만 적용 (빈자리 우선 없음)
            List<int> candidates = GetAdjacencyValidColumns_Soft(_heights, totalLayers);

            int chosen;
            if (candidates.Count > 0)
            {
                chosen = candidates[Random.Range(0, candidates.Count)];
            }
            else
            {
                // 폴백: 인접 불균형 최소화 + cap 준수
                chosen = ChooseLeastImbalanceColumnAdj(_heights, totalLayers);
            }

            Transform dropPoint = spawnPoints[chosen];

            // 경고 마커
            Vector3 groundPos = GetGroundPoint(dropPoint.position);
            GameObject marker = null;
            if (warnMarkerPrefab != null)
                marker = Instantiate(warnMarkerPrefab, groundPos + warnMarkerOffset, Quaternion.identity);

            // 경고 유지
            if (preWarnTime > 0f)
                yield return new WaitForSeconds(preWarnTime);

            // 낙석 생성
            Instantiate(rockPrefab, dropPoint.position, Quaternion.identity);

            // 높이 갱신
            _heights[chosen]++;

            // 마커 제거
            if (marker != null) Destroy(marker);

            // 다음 낙석까지 대기
            if (k < totalDrops - 1 && postSpawnWait > 0f)
                yield return new WaitForSeconds(postSpawnWait);
        }

        _spawnRoutine = null;
    }

    /// <summary>
    /// SOFT 모드: 마지막 층 '빈자리 우선' 규칙 없음.
    /// - cap: 각 열은 totalLayers 초과 금지
    /// - 인접 제약: 투하 결과가 |h0-h1|<=1 && |h1-h2|<=1 여야 함
    /// </summary>
    private List<int> GetAdjacencyValidColumns_Soft(int[] h, int maxLayers)
    {
        var eligible = new List<int>(3);

        int h0 = h[0], h1 = h[1], h2 = h[2];

        for (int i = 0; i < 3; i++)
        {
            if (h[i] >= maxLayers) continue; // cap

            int a0 = h0, a1 = h1, a2 = h2;
            if (i == 0) a0++;
            else if (i == 1) a1++;
            else a2++;

            bool ok = (Mathf.Abs(a0 - a1) <= 1) && (Mathf.Abs(a1 - a2) <= 1);
            if (ok) eligible.Add(i);
        }

        return eligible;
    }

    /// <summary>
    /// 폴백: 후보가 전혀 없을 때(희박하지만 안전장치)
    /// - cap 준수
    /// - 인접 불균형 max(|h0-h1|, |h1-h2|) 최소화
    /// - 동률이면 더 낮은 열 우선
    /// </summary>
    private int ChooseLeastImbalanceColumnAdj(int[] h, int maxLayers)
    {
        int bestIdx = -1;
        int bestScore = int.MaxValue;

        int h0 = h[0], h1 = h[1], h2 = h[2];

        for (int i = 0; i < 3; i++)
        {
            if (h[i] >= maxLayers) continue; // cap

            int a0 = h0, a1 = h1, a2 = h2;
            if (i == 0) a0++; else if (i == 1) a1++; else a2++;

            int score = Mathf.Max(Mathf.Abs(a0 - a1), Mathf.Abs(a1 - a2));
            if (score < bestScore || (score == bestScore && h[i] < (bestIdx >= 0 ? h[bestIdx] : int.MaxValue)))
            {
                bestScore = score;
                bestIdx = i;
            }
        }

        if (bestIdx == -1)
        {
            int minH = int.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                if (h[i] >= maxLayers) continue;
                if (h[i] < minH) { minH = h[i]; bestIdx = i; }
            }
        }

        return (bestIdx == -1) ? 0 : bestIdx;
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
