using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    public void Respawn()
    {
        if (!RespawnPoint.IsSet)
        {
            Debug.LogWarning("[Player] 리스폰 위치가 설정되지 않았습니다.");
            return;
        }
        
        transform.position = RespawnPoint.RespawnPosition;
        Debug.Log($"[Player] 리스폰 위치로 이동: {RespawnPoint.RespawnPosition}");
    }
}
