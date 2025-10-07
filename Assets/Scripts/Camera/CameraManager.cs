using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

public enum ECameraState
{
    Targeting = 0, //default
    StopAtPosition,
}

//기능이 많아진다 싶으면 아예 FSM으로 관리 아직은 현상유지(0923)
public class CameraManager : SceneSingleton<CameraManager>
{
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private float followSpeed = 5f;
    private const float cameraZ = -10f;
    
    //상태관리
    private Transform target;
    private ECameraState _state = ECameraState.Targeting;
    private Vector2 _point;
    
    void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        transform.position = GetDesiredPosition();
    }
    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPos = GetDesiredPosition();
        Vector3 targetPoint = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);
        transform.position = new Vector3(targetPoint.x, targetPoint.y, cameraZ);
    }

    private Vector3 GetDesiredPosition()
    {
        return _state switch
        {
            ECameraState.Targeting => target.position + targetOffset,
            ECameraState.StopAtPosition => _point,
            _ => transform.position
        };
    }

    public void StopCameraAtPoint(Vector2 pos)
    {
        _point = pos;
        _state = ECameraState.StopAtPosition;
    }

    public void FollowTarget()
    {
        _state = ECameraState.Targeting;
    }
}
