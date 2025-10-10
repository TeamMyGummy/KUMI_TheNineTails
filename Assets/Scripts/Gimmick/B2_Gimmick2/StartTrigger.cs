using System.Collections;
using UnityEngine;
using UnityEngine.Events; // UnityEvent를 사용하기 위해 꼭 추가해야 합니다!

public class ImoogiTrigger : MonoBehaviour
{
    // --- 수정된 부분: UnityEvent를 사용합니다 ---
    // 인스펙터에서 플레이어가 닿았을 때 실행할 함수들을 연결할 수 있습니다.
    [SerializeField]
    public UnityEvent onPlayerEnter;
    // --- 여기까지 ---
    
    PlayerController _playerController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController = other.GetComponent<PlayerController>();
            if (_playerController == null)
            {
                return; 
            }
            
            Debug.Log("플레이어가 트리거에 닿았습니다! 이벤트를 발생시킵니다.");

            // --- 수정된 부분: 직접 함수를 호출하는 대신 이벤트를 발생시킵니다 ---
            // 인스펙터에 연결된 모든 함수들이 여기서 실행됩니다.
            if (onPlayerEnter != null)
            {
                onPlayerEnter.Invoke();
            }
            // --- 여기까지 ---
            
            _playerController.OnDisableAllInput();
            StartCoroutine(WaitSeconds(3f));
        }
    }
    
    private IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _playerController.OnEnableAllInput();
        gameObject.SetActive(false);
        
        SoundManager.Instance.PlayBGM(BGMName.B2_Gimmick2);
    }
}