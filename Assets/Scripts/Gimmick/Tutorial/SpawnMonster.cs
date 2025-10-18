using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour{

    public GameObject monsterPrefab; // 몬스터 프리팹을 유니티 에디터에서 할당
    public Transform spawnPoint;     // 몬스터가 생성될 위치를 지정 (빈 오브젝트를 생성하여 위치 지정)

    private GameObject currentMonster; // 현재 생성된 몬스터를 추적하기 위한 변수
    private bool isMonsterSpawnable = false; // 몬스터 생성 가능 여부를 제어하는 플래그

    void Update()
    {
        // 현재 몬스터가 없고, 생성이 가능한 상태일 때만 몬스터를 생성
        if (isMonsterSpawnable && currentMonster == null)
        {
            SpawningMonster();
        }
    }

    /// <summary>
    /// 몬스터를 생성하는 함수
    /// </summary>
    private void SpawningMonster()
    {
        if (monsterPrefab != null && spawnPoint != null)
        {
            currentMonster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("몬스터가 스폰되었습니다!");
        }
        else
        {
            Debug.LogError("몬스터 프리팹 또는 스폰 위치가 지정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 몬스터 생성을 시작하는 이벤트 함수
    /// </summary>
    public void StartSpawning()
    {
        isMonsterSpawnable = true;
        Debug.Log("몬스터 스폰을 시작합니다.");
    }

    /// <summary>
    /// 몬스터 생성을 중지하는 이벤트 함수
    /// </summary>
    public void StopSpawning()
    {
        isMonsterSpawnable = false;
        Debug.Log("몬스터 스폰을 중지합니다.");

        // 스폰 중지 시 현재 몬스터도 제거하고 싶다면 아래 주석을 해제하세요.
        // if (currentMonster != null)
        // {
        //     Destroy(currentMonster);
        // }
    }
}

