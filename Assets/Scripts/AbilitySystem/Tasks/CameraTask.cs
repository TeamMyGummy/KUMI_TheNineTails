using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera 움직임 관련 Task 커스터마이징
/// </summary>
public abstract class CameraTask : ScriptableObject
{
    protected Camera Camera;

    /// <summary>
    /// Task 생성자 -> 카메라를 반드시 넘겨주어야 함
    /// </summary>
    /// <param name="camera">Task 대상이 되는 카메라</param>
    /// <param name="duration">Task 지속 시간</param>
    public virtual void Initialize(Camera camera)
    {
        this.Camera = camera;
    }
    
    /// <summary>
    /// Task가 실질적으로 실행되는 부분
    /// </summary>
    public abstract void Execute();
    
    /// <summary>
    /// Task가 종료되었을 때 처리
    /// </summary>
    public abstract void EndTask();
}
