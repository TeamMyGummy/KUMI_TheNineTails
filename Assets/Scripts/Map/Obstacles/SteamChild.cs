using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamChild : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 1.0f;
    
    private SteamObject _parentSteam;
    private bool _isProcessing = false;
    

    private void Awake()
    {
        _parentSteam = GetComponentInParent<SteamObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (_parentSteam != null && _parentSteam.IsActive())
        {
            collision.GetComponent<Damageable>()?.GetDamage(DomainKey.Player, _parentSteam.GetDamage());
            
            var controller = collision.GetComponent<PlayerRespawnController>();
            var pc = collision.GetComponent<PlayerController>();
            if (controller != null)
            {
                _isProcessing = true;
                StartCoroutine(RespawnAfterDelay(controller, pc));
            }
        }
    }
    
    // 장애물에 닿으면 딜레이 후 리스폰
    private IEnumerator RespawnAfterDelay(PlayerRespawnController controller, PlayerController pc)
    {
        pc.OnDisableAllInput(); //플레이어 이동 막기
        yield return new WaitForSeconds(respawnDelay);
        controller.Respawn();
        pc.OnEnableAllInput();
    }
}