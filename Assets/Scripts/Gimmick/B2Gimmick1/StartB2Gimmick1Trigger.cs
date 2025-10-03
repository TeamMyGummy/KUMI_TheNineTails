using System.Collections;
using UnityEngine;



public class StartB2Gimmick1Trigger : MonoBehaviour
{
    public bool isTriggerFinished = false;

    
    PlayerController _playerController;
    public Water _water;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController = other.GetComponent<PlayerController>();
            if (_playerController == null) return;

            Debug.Log("플레이어가 트리거에 닿았습니다! 물이 차오릅니다.");

            StartCoroutine(GimmickSequence());
        }
    }

    private IEnumerator GimmickSequence()
    {
        _playerController.OnDisableAllInput();

        _water.StartRise(2f); // 2초 동안 상승
        yield return new WaitForSeconds(2f);

        _water.StopRise(3f); // 3초 멈춤
        yield return new WaitForSeconds(3f);

        _playerController.OnEnableAllInput();

        _water.StartRise(float.MaxValue); // 이후 계속 상승
        gameObject.SetActive(false);
    }

}