using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

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
    
    [Header("Shake Settings")]
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
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
    
    public void Shake(float intensity, float duration)
    {
        ShakeAsync(transform.localPosition, intensity, duration).Forget();
    }
    
    private async UniTask ShakeAsync(Vector3 originPos, float intensity, float duration)
    {
        float elapsed = 0f;
        
        try
        {
            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float curveValue = shakeCurve.Evaluate(progress);
                
                float x = Random.Range(-1f, 1f) * intensity * curveValue;
                float y = Random.Range(-1f, 1f) * intensity * curveValue;

                transform.localPosition = originPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        catch (OperationCanceledException)
        {
            // 예외 무시
        }
        finally
        {
            transform.localPosition = originPos;
        }
    }
}
