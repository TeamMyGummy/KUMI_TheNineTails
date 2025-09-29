using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시각적/물리적 표현 담당: 밟는 콜라이더 on/off만 처리 (애니/이펙트는 나중에 추가)
/// </summary>
public class PlatformView : MonoBehaviour
{
    private Collider2D solidCollider; // 플레이어가 실제로 서는 콜라이더
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        // 자동 할당: 인스펙터에 안 넣어도 해당 게임오브젝트에서 찾아서 넣음
        if (!solidCollider) solidCollider = GetComponent<Collider2D>();  // ← 수정된 부분
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();  // ← 수정된 부분
    }
    
    public void ShowOpen()
    {
        if (solidCollider) solidCollider.enabled = true;
        if (spriteRenderer) spriteRenderer.enabled = true; // 보이게
    }

    public void ShowClosed()
    {
        if (solidCollider) solidCollider.enabled = false;
        if (spriteRenderer) spriteRenderer.enabled = false; // 안 보이게
    }
}

