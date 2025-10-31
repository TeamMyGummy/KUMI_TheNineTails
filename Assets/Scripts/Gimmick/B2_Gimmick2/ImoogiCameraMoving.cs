using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImoogiCameraMoving : MonoBehaviour
{
    [SerializeField] private Vector2 position;

    public void StartMoving()
    {
        CameraManager.Instance.StopCameraAtPoint(position);
        StartCoroutine(Shaking());
        StartCoroutine(FollowPlayer());
    }

    public void ShakingCamera()
    {
        CameraManager.Instance.Shake(0.7f, 0.2f);
    }

    private IEnumerator FollowPlayer()
    {
        yield return new WaitForSeconds(3f);
        CameraManager.Instance.FollowTarget();
    }

    private IEnumerator Shaking()
    {
        yield return new WaitForSeconds(2f);
        CameraManager.Instance.Shake(1f, 1.5f);
    }

    private void RandomSFX()
    {
        int randomIndex = Random.Range(0, 3);

        switch (randomIndex)
        {
            case 0:
                SoundManager.Instance.PlaySFX(SFXName.이무기벽쿵1);
                break; // 0일 때 Action1() 실행
            case 1:
                SoundManager.Instance.PlaySFX(SFXName.이무기벽쿵2);
                break; // 1일 때 Action2() 실행
            case 2:
                SoundManager.Instance.PlaySFX(SFXName.이무기벽쿵3);
                break; // 2일 때 Action3() 실행
            default:
                break;
        }
    }
}
