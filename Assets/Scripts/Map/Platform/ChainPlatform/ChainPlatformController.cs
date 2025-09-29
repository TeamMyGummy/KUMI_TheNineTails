using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체인(0..n-1)의 상태/방향/릴레이를 관리.
/// - direction: 0=대기(모두 닫힘), +1=정방향, -1=역방향
/// - 어떤 플랫폼이 닫히면 idx+direction을 연다(범위 내일 때)
/// - 모두 닫히면 direction=0으로 리셋
/// </summary>
public class PlatformChainController : MonoBehaviour
{
    [Tooltip("진행 순서대로 등록 (0..n-1). 0=첫 플랫폼, n-1=마지막 플랫폼")]
    public RelayPlatform[] platforms;

    private int openCount = 0;
    private int direction = 0; // 0=idle, +1=forward, -1=backward

    private void Awake()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            RelayPlatform p = platforms[i];

            // 이벤트 구독
            p.OnOpened += _ => openCount++;
            p.OnClosed += _ =>
            {
                openCount = Mathf.Max(0, openCount - 1);
                if (openCount == 0) direction = 0; // 모두 닫히면 대기 상태로
            };

            // 역참조 주입
            p.Chain = this;

            // 양 끝 Proximity라면 인덱스 정보 제공
            if (p is ProximityPlatform prox) prox.MyIndexInChain = i;
        }
    }

    public bool AllClosed() => openCount == 0;
    public bool IsFirst(int idx) => idx == 0;
    public bool IsLast(int idx)  => idx == platforms.Length - 1;

    /// <summary>양 끝 Proximity가 시작될 때 방향을 설정</summary>
    public void SetDirection(int dir)
    {
        direction = Mathf.Clamp(dir, -1, 1);
    }

    /// <summary>플랫폼이 닫힐 때 호출: 방향에 따라 다음 인덱스를 연다</summary>
    public void OpenNextAfter(RelayPlatform prev)
    {
        if (direction == 0) return; // 대기 상태면 진행 안 함

        int idx = System.Array.IndexOf(platforms, prev);
        if (idx < 0) return;

        int next = idx + direction;
        if (next >= 0 && next < platforms.Length)
        {
            platforms[next].Open();
        }
        // 범위를 벗어나면 끝에 도달한 것. 더 이상 열지 않음.
        // (이후 전부 닫히면 direction은 0으로 자동 리셋됨)
    }
}

