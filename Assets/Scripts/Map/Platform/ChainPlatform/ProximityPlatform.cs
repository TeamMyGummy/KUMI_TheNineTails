using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 양 끝(첫/마지막) 플랫폼 전용: "근접 시 시작" 로직.
/// - 모두 닫힘 상태에서만 근접으로 Open 가능
/// - Open될 때 자신이 '첫/마지막'인지에 따라 체인의 진행 방향을 설정
/// </summary>
public class ProximityPlatform : RelayPlatform
{
    private bool playerInRange = false;

    // 컨트롤러가 Awake에서 채워줌
    public int MyIndexInChain { get; set; } = -1;

    /// <summary>
    /// ProximityRange(자식)에서 들어옴/나감 이벤트를 올려주는 엔트리 포인트
    /// </summary>
    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;

        // "모두 닫힘" 상태(대기)에서만, 플레이어가 근접했을 때 시작
        if (playerInRange && Chain != null && Chain.AllClosed())
        {
            Open(); // 먼저 열고
            // 자신이 첫/마지막인지에 따라 방향 결정
            if (Chain.IsFirst(MyIndexInChain))      Chain.SetDirection(+1);
            
            else if (Chain.IsLast(MyIndexInChain))  Chain.SetDirection(-1);
        }
    }
}

