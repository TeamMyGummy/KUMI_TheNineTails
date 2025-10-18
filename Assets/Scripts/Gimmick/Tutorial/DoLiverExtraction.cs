using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoLiverExtraction : MonoBehaviour
{
    [SerializeField] private ConditionKey condition;
    
    [SerializeField] private Image[] boxesToFill; // Inspector에서 채울 네모 칸 이미지들을 연결
    [SerializeField] private Color filledColor = Color.cyan; // 채워졌을 때의 색상

    private int currentIndex = 0; // 현재 채워야 할 칸의 인덱스
    private int _liverCnt = 0;
    private int _requiredCnt = 4;

    
    private void Awake()
    {
        currentIndex = 0; // 인덱스 초기화

        // 모든 박스를 기본 색상으로 리셋 (선택 사항)
        foreach (var box in boxesToFill)
        {
            box.color = Color.white; // 기본 색상으로 설정
        }

        FindAnyObjectByType<SpawnMonster>().StartSpawning();
    }

    private void FillNextBox()
    {
        // 아직 채울 칸이 남아있다면
        if (currentIndex < boxesToFill.Length)
        {
            // 현재 인덱스의 박스 색상을 변경
            boxesToFill[currentIndex].color = filledColor;
            currentIndex++; // 다음 칸으로 인덱스 이동

            // 모든 칸을 다 채웠는지 확인
            if (currentIndex >= boxesToFill.Length)
            {
                ConditionEventBus.Raise(condition);
            }
        }
    }

    public void RecordLiverSuccess()
    {
        OnLiverSuccess();
        FillNextBox();
    }

    private void OnLiverSuccess()
    {
        _liverCnt++;
        if (_liverCnt >= _requiredCnt)
        {
            FindAnyObjectByType<SpawnMonster>().StopSpawning();
        }
    }
}
