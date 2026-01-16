using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImoogiCameraMoving : MonoBehaviour
{
    [SerializeField] private Vector2 position;

    public void StartMoving()
    {
        CameraManager.Instance.StopCameraAtPoint(position);
        StartCoroutine(FollowTarget());
    }

    public void ShakingCamera()
    {
        CameraManager.Instance.Shake(0.7f, 0.2f);
    }

    private IEnumerator FollowTarget()
    {
        yield return new WaitForSeconds(2f);
        CameraManager.Instance.Shake(0.7f, 0.2f);
        CameraManager.Instance.FollowTarget();
    }

    private IEnumerator Shaking()
    {
        yield return new WaitForSeconds(0f);
        CameraManager.Instance.Shake(0.7f, 1f);
        RandomSFX();
    }

    private void RandomSFX()
    {
        int randomIndex = Random.Range(0, 3);

        switch (randomIndex)
        {
            case 0:
                SoundManager.Instance.PlaySFX(SFXName.이무기_기믹2_벽쿵_1);
                break; // 0일 때 Action1() 실행
            case 1:
                SoundManager.Instance.PlaySFX(SFXName.이무기_기믹2_벽쿵_2);
                break; // 1일 때 Action2() 실행
            case 2:
                SoundManager.Instance.PlaySFX(SFXName.이무기_기믹2_벽쿵_3);
                break; // 2일 때 Action3() 실행
            default:
                break;
        }
    }
}
