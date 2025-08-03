using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "AbilityTask/Camera/CameraZoomIn")]
public class CameraZoomIn : CameraTask
{
    public float _zoomInSize;
    public float _duration;            // 줌인 시간
    
    public override void Execute()
    {
       Camera?.DOOrthoSize(_zoomInSize, _duration).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
    }

    public override void EndTask()
    {
        
    }
}
