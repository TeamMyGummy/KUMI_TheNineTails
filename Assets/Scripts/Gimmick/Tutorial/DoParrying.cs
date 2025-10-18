using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoParrying : MonoBehaviour
{
    [SerializeField] private ConditionKey condition;
    
    [SerializeField] private Image[] boxesToFill; // Inspector에서 채울 네모 칸 이미지들을 연결
    [SerializeField] private Color filledColor = Color.cyan; // 채워졌을 때의 색상
    [SerializeField] private SpawnMonster spawnMonster;

    [SerializeField] private GameObject liverPopup;
	[SerializeField] private GameObject prevPopup;
    
    private int currentIndex = 0; // 현재 채워야 할 칸의 인덱스

    private bool _isDoorOpened = false;
    private int _parryCount = 0;
    private int RequiredParries = 4;

    private SpawnMonster _obj;
    
    private void Awake()
    {
        currentIndex = 0; // 인덱스 초기화

        // 모든 박스를 기본 색상으로 리셋 (선택 사항)
        foreach (var box in boxesToFill)
        {
            box.color = Color.white; // 기본 색상으로 설정
        }

        spawnMonster.StartSpawning();
    }
    
    /// <summary>
    /// Hitbox에서 호출할 static 함수입니다.
    /// 패링이 성공하면 이 함수를 통해 카운트를 증가시킵니다.
    /// </summary>
    public void RecordParrySuccess()
    {
        OnParrySuccess();
        FillNextBox();
    }
    
    /// <summary>
    /// 실제 패링 카운트를 처리하는 내부 함수입니다.
    /// </summary>
    private void OnParrySuccess()
    {
        if (_isDoorOpened) return;

        _parryCount++;
        Debug.Log($"패링 성공! 카운트: {_parryCount} / {RequiredParries}");

        if (_parryCount >= RequiredParries)
        {
            _isDoorOpened = true;
			prevPopup.SetActive(false);
            liverPopup.SetActive(true);
        }
    }
    
    private void FillNextBox()
    {
        // 아직 채울 칸이 남아있다면
        if (currentIndex < boxesToFill.Length)
        {
            // 현재 인덱스의 박스 색상을 변경
            boxesToFill[currentIndex].color = filledColor;
            currentIndex++; // 다음 칸으로 인덱스 이동

            
        }
    }
}
