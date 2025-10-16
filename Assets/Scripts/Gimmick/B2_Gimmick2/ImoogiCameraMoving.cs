using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImoogiCameraMoving : MonoBehaviour
{
    [SerializeField] private Vector2 position;

    public void StartMoving()
    {
        CameraManager.Instance.StopCameraAtPoint(position);
        StartCoroutine(FollowPlayer());
    }

    private IEnumerator FollowPlayer()
    {
        yield return new WaitForSeconds(3f);
        CameraManager.Instance.FollowTarget();
    }
}
