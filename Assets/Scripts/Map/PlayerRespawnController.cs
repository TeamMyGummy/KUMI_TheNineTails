using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [SerializeField] private GameObject respawnGround;

    private Vector2 _respawnPosition;
    private GameObject _col;
    /*private float x=0f, y=0f;*/
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RespawnPoint"))
        {
            Transform yTransform = other.transform.Find("Y");
            
            if (yTransform == null)
            {
                Debug.LogError("[RespawnPoint] 자식 오브젝트 'Y'를 찾을 수 없습니다.");
                return;
            }
            
            BoxCollider2D yCol = yTransform.GetComponent<BoxCollider2D>();
            if (yCol == null)
            {
                Debug.LogError("[RespawnPoint] 'Y' 오브젝트에 BoxCollider2D가 없습니다.");
                return;
            }

            float y = yCol.bounds.max.y;
            float x = other.bounds.center.x;
            
            _respawnPosition = new Vector2(x, y);
            Debug.Log($"[RespawnPoint] 저장된 위치: {_respawnPosition}");
        }
    }
    
    public void Respawn()
    {
        transform.position = _respawnPosition;
        Debug.Log($"[Player] 리스폰 위치로 이동: {_respawnPosition}");
    }
}
