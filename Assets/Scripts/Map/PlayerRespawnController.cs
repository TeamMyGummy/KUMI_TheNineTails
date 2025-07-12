using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    public void Respawn()
    {
        transform.position = RespawnPoint.RespawnPosition;
        Debug.Log($"[Player] 리스폰 위치로 이동: {RespawnPoint.RespawnPosition}");
    }
}
