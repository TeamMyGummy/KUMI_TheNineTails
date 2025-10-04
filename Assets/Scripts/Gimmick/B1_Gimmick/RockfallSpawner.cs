using System.Collections;
using UnityEngine;

public class RockfallSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private Transform[] spawnPoints = new Transform[3];
    [SerializeField] private GameObject rockPrefab;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float spawnInterval = 3f; // 낙석 간격
    /*[SerializeField, Min(0f)] private float interLayerDelay = 3f; // 층-사이 간격*/

    [Header("Layers")]
    [SerializeField, Min(1)] private int totalLayers = 5;

    [Header("Warning Marker")]
    [SerializeField, Min(0f)] private float preWarnTime = 1f;            // 경고가 먼저 뜨는 시간
    [SerializeField] private GameObject warnMarkerPrefab;                // 빨간 타원 프리팹(스프라이트/이미지)
    [SerializeField] private LayerMask groundMask;                       // 바닥 레이어
    [SerializeField, Min(0.1f)] private float raycastDistance = 50f;     // 아래로 쏠 거리
    [SerializeField] private Vector3 warnMarkerOffset = new Vector3(0, 0.05f, 0); // 살짝 위로 띄우기

    [Header("Start")]
    [SerializeField] private bool autoStartOnPlay = true;

    private Coroutine _spawnRoutine;

    private void Start()
    {
        if (autoStartOnPlay) StartSpawning();
    }

    public void StartSpawning()
    {
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        _spawnRoutine = StartCoroutine(SpawnLayers());
    }

    private IEnumerator SpawnLayers()
    {
        // 기본 검증
        if (rockPrefab == null || spawnPoints.Length != 3 ||
            spawnPoints[0] == null || spawnPoints[1] == null || spawnPoints[2] == null)
        {
            Debug.LogError("[RockfallSpawner] 설정이 올바르지 않습니다.");
            yield break;
        }

        // preWarnTime이 spawnInterval보다 길면, 다음 간격 계산에서 0 이하가 될 수 있으니 안전하게 보정
        float postSpawnWait = Mathf.Max(0f, spawnInterval - preWarnTime);

        for (int layer = 0; layer < totalLayers; layer++)
        {
            int[] order = { 0, 1, 2 };
            Shuffle(order);

            for (int i = 0; i < order.Length; i++)
            {
                Transform dropPoint = spawnPoints[order[i]];

                // 1) 바닥 위치 계산(아래로 Raycast)
                Vector3 groundPos = GetGroundPoint(dropPoint.position);

                // 2) 경고 마커 생성 (떨어지기 preWarnTime초 전)
                GameObject marker = null;
                if (warnMarkerPrefab != null)
                {
                    marker = Instantiate(warnMarkerPrefab, groundPos + warnMarkerOffset, Quaternion.identity);
                }

                // 3) 경고 노출 시간 대기
                if (preWarnTime > 0f)
                    yield return new WaitForSeconds(preWarnTime);

                // 4) 낙석 생성
                Instantiate(rockPrefab, dropPoint.position, Quaternion.identity);

                // 5) 마커 제거(즉시 or 페이드 아웃이 있다면 프리팹에서 처리)
                if (marker != null) Destroy(marker);

                // 6) 같은 층 내 다음 낙석까지 대기(“경고 포함 전체 간격”이 spawnInterval)
                bool isLastOfLayer = (i == order.Length - 1);
                if (!isLastOfLayer && postSpawnWait > 0f)
                    yield return new WaitForSeconds(postSpawnWait);
            }

            // 7) 층 간 대기
            bool isLastLayer = (layer == totalLayers - 1);
            // 기존: 층 끝나면 전부 기다림
            
            // if (!isLastLayer && interLayerDelay > 0f)
            //     yield return new WaitForSeconds(interLayerDelay);

            // 변경: 다음 경고가 뜨기 전에만 일부를 대기
            float gapToNextWarning = Mathf.Max(0f, spawnInterval - preWarnTime);
            if (!isLastLayer && gapToNextWarning > 0f)
                yield return new WaitForSeconds(gapToNextWarning);

            // 다음 for-루프가 시작되면 "경고 마커 생성"이 즉시 실행됨
            // (그 다음 preWarnTime 동안 표시 → 낙석 생성)
        }

        _spawnRoutine = null;
    }

    private Vector3 GetGroundPoint(Vector3 from)
    {
        // 2D 물리 기준: Physics2D.Raycast 사용
        RaycastHit2D hit = Physics2D.Raycast(from, Vector2.down, raycastDistance, groundMask);
        if (hit.collider != null)
            return hit.point;
        // 못 맞추면 그냥 y만 약간 내려서 반환(프로젝트에 맞게 고정 Y를 쓰고 싶다면 여기 수정)
        return from + Vector3.down * 1.0f;
    }

    // Fisher–Yates
    private void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
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
