using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 중간/끝 공용: "밟으면 닫힘", "닫히면 다음을 연다"(다음 인덱스는 체인의 방향에 의존)
/// 양 끝은 ProximityPlatform이 이 클래스를 상속받아 기능을 추가.
/// </summary>
public class RelayPlatform : MonoBehaviour
{
    /*[Header("References")]*/
    protected PlatformView view;
    protected Collider2D stepTrigger; // 얇은 윗면 트리거(Enter=밟음)

    public PlatformChainController Chain { get; set; } // 체인 매니저(런타임 주입)
    public bool IsOpen { get; private set; }

    public event Action<RelayPlatform> OnOpened;
    public event Action<RelayPlatform> OnClosed;
    
    private int playerContacts = 0;

    protected virtual void Awake()
    {
        if (view == null) view = GetComponentInChildren<PlatformView>();
        if (stepTrigger == null) stepTrigger = GetComponentInChildren<Collider2D>();
        
        SetOpen(false); // 시작은 닫힘
    }

    public virtual void Open()  => SetOpen(true);
    public virtual void Close() => SetOpen(false);

    protected void SetOpen(bool open)
    {
        if (IsOpen == open) return;

        IsOpen = open;
        if (open) view.ShowOpen(); else view.ShowClosed();

        if (open) OnOpened?.Invoke(this);
        else
        {
            OnClosed?.Invoke(this);
            // 닫히는 순간 체인에게 "다음 열기" 요청 (방향/범위 체크는 컨트롤러가 담당)
            Chain?.OpenNextAfter(this);
        }
    }

    // 밟기 시작: 카운트만 올림 (닫지 않음)
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerContacts++;
    }

    // 완전히 떠날 때: 카운트가 0이 되는 순간에만 닫음
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerContacts = Mathf.Max(0, playerContacts - 1);

        if (playerContacts == 0 && IsOpen)
        {
            Close(); // ← 여기서 다음 플랫폼 오픈 체인이 이어짐
        }
    }
}

