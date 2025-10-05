using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
    [SerializeField]
        private List<Vector3> points;

    [SerializeField] private List<float> segmentsSpeed; // 각 세그먼트의 속도를 저장할 리스트
    
        public int PointCount => points.Count;
        public int SegmentCount => (points.Count - 1) / 3;
    
        public Vector3 this[int i]
        {
            get => transform.TransformPoint(points[i]);
            set => points[i] = transform.InverseTransformPoint(value);
        }
    
        private void Reset()
        {
            points = new List<Vector3>
            {
                new Vector3(1, 0, 0),
                new Vector3(2, 0, 0),
                new Vector3(3, 0, 0),
                new Vector3(4, 0, 0)
            };

            segmentsSpeed = new List<float> { 5f };
        }
    
        public void AddSegment(Vector3 anchorPos)
        {
            // 마지막 점을 기준으로 새로운 세그먼트를 추가합니다.
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
            points.Add(anchorPos);
            
            segmentsSpeed.Add(segmentsSpeed[segmentsSpeed.Count - 1]);
        }

        public float GetSpeed(int segmentIndex)
        {
            return segmentsSpeed[Mathf.Clamp(segmentIndex, 0, segmentsSpeed.Count - 1)];
        }
    
        public Vector3[] GetPointsInSegment(int i)
        {
            return new Vector3[] { this[i * 3], this[i * 3 + 1], this[i * 3 + 2], this[i * 3 + 3] };
        }
        
        // 전체 경로(0~1)에서의 t값으로 위치를 반환합니다.
        public Vector3 GetPoint(float t)
        {
            int segmentIndex;
            if (t >= 1f)
            {
                t = 1f;
                segmentIndex = SegmentCount - 1;
            }
            else
            {
                t = Mathf.Clamp01(t) * SegmentCount;
                segmentIndex = (int)t;
                t -= segmentIndex;
            }
    
            Vector3[] segmentPoints = GetPointsInSegment(segmentIndex);
            return Bezier.EvaluateCubic(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3], t);
        }
    }
    
    // 베지어 계산을 위한 static 클래스
    public static class Bezier
    {
        public static Vector3 EvaluateCubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * p0 +
                   3f * oneMinusT * oneMinusT * t * p1 +
                   3f * oneMinusT * t * t * p2 +
                   t * t * t * p3;
        }
}
