using BehaviorTree;
using UnityEngine;

namespace BehaviorTree.Leaf
{
    public class CurveMoveNode : MonoNode
    {
        [Header("목표 지점")]
        [SerializeField] private float x;
        [SerializeField] private EMoveType xType;
        [SerializeField] private float y;
        [SerializeField] private EMoveType yType;

        [Header("이동 설정")]
        [Tooltip("목표 지점까지 이동하는 데 걸리는 시간입니다.")]
        [SerializeField] private float duration = 1f;
        [Tooltip("움직임의 속도/가속도를 조절하는 커브입니다. 비워두면 등속(Linear)으로 움직입니다.")]
        [SerializeField] private AnimationCurve curve;

        protected override void OnEnter()
        {
            if (runtimeHandler is CurveMoveHandler handler) 
            {
                var targetVector = new Vector3(x, y, 0f);
                handler.SetMovementPoint(targetVector, xType, yType, duration, curve);
            }
        }
    }
}