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
}
