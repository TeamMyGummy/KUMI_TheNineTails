using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckClimbing : MonoBehaviour
{
    [SerializeField] private ConditionKey condition;
    [SerializeField] private Image progressBar;
    [Header("설정")]
    [SerializeField] private float requiredHoldTime = 3f; // 키를 누르고 있어야 하는 시간

    // 내부 변수
    private PlayerController playerController; // 플레이어 컨트롤러 참조
    private Player player; // 플레이어 상태 머신 참조
    private float climbHoldTimer = 0f;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        player = FindObjectOfType<Player>();
    }

    

    private void Update()
    {
        // 아래 조건 중 하나라도 만족하면 함수 종료
        if (player == null)
        {
            Debug.Log("안됨");
            return;
        }

        // 1. 플레이어가 벽타기 상태인지 확인
        // (Player 스크립트에 StateMachine이 있고, IsCurrentState 메서드가 있다고 가정)
        bool isWallClimbing = player.StateMachine.IsCurrentState(PlayerStateType.WallClimb);

        // 2. W 또는 S 키를 누르고 있는지 확인
        // PlayerController의 ClimbInput 값을 사용
        bool isHoldingClimbKey = playerController.ClimbInput.y > 0.5f || playerController.ClimbInput.y < -0.5f;

        // 3. 두 조건이 모두 참이면 타이머 증가
        if (isWallClimbing && isHoldingClimbKey)
        {
            climbHoldTimer += Time.deltaTime;
            progressBar.fillAmount = climbHoldTimer / requiredHoldTime;

            // 4. 타이머가 목표 시간에 도달하면 문 열기
            if (climbHoldTimer >= requiredHoldTime)
            {
                ConditionEventBus.Raise(condition);
            }
        }
        
    }
}
