using System.Collections;
using UnityEngine;

public class ImoogiTrigger : MonoBehaviour
{
    // 인스펙터에서 움직이게 할 이무기(PathFollower 스크립트)를 연결해줍니다.
    [SerializeField]
    private PathFollower imoogiToActivate;
    
    PlayerController _playerController;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 1. 부딪힌 'other' 오브젝트에서 PlayerController 컴포넌트를 찾습니다.
            _playerController = other.GetComponent<PlayerController>();

            // 2. PlayerController가 실제로 있는지 확인합니다. (안전장치)
            if (_playerController == null)
            {
                // PlayerController가 없으면 그냥 함수를 종료합니다.
                return; 
            }
            
            Debug.Log("플레이어가 트리거에 닿았습니다! 이무기를 깨웁니다.");

            if (imoogiToActivate != null)
            {
                imoogiToActivate.StartMoving();
            }
            
            _playerController.OnDisableAllInput();

            StartCoroutine(WaitSeconds(3f));
            
            
        }
    }
    
    private IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _playerController.OnEnableAllInput();
        gameObject.SetActive(false);
    }
}