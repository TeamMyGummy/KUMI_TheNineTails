using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public static Vector2 RespawnPosition{ get; private set; }
    private BoxCollider2D _col;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();   
        if (_col == null)
        {
            Debug.LogError("[RespawnPoint] BoxCollider2D가 없습니다");
            return;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPosition = new Vector2(transform.position.x, _col.bounds.max.y);
            Debug.Log($"[RespawnPoint] 저장된 위치: {RespawnPosition}");
        }
    }
}
