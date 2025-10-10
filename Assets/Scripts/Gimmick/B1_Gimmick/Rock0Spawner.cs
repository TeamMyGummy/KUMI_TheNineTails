using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock0Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject rockPrefab;     // Rock0 프리팹
    [SerializeField, Min(0)] private int count = 5;     // n개
    [SerializeField] private float stepX = 1f;          // x축 간격 (칸)
    [SerializeField, Min(0f)] private float interval = 0.25f; // 생성 간격(초)

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;   // 시작 시 자동 생성
    [SerializeField] private bool clearChildrenBeforeSpawn = false; // 기존 자식 제거

    private Coroutine _spawnRoutine;

    private void Start()
    {
        if (playOnStart)
            StartSpawn();
    }

    [ContextMenu("Start Spawn")]
    public void StartSpawn()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        if (clearChildrenBeforeSpawn)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        _spawnRoutine = StartCoroutine(SpawnCo());
    }

    [ContextMenu("Stop Spawn")]
    public void StopSpawn()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnCo()
    {
        if (rockPrefab == null)
        {
            Debug.LogWarning("[RockSpawner] rockPrefab이 비어있습니다.");
            yield break;
        }

        // ✅ 스포너의 현재 위치를 StartPoint로 사용
        Vector3 startPoint = transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = startPoint + new Vector3(stepX * i, 0f, 0f);
            GameObject go = Instantiate(rockPrefab, pos, Quaternion.identity, transform);
            go.name = $"{rockPrefab.name}_{i}";

            if (interval > 0f)
                yield return new WaitForSeconds(interval);
            else
                yield return null;
        }

        _spawnRoutine = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 startPoint = transform.position;
        for (int i = 0; i < Mathf.Max(count, 0); i++)
        {
            Vector3 pos = startPoint + new Vector3(stepX * i, 0f, 0f);
            Gizmos.DrawWireSphere(pos, 0.15f);
        }
    }
#endif
}
