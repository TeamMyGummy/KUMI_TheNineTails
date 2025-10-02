using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField]
    private BezierPath path; // 따라갈 경로

    [SerializeField]
    private bool lookForward = true; // 이동 방향을 바라보게 할지 여부
    
    private float t = 0f; // 전체 경로 상의 위치 (0.0 ~ 1.0)
    private bool canMove = false; // 이무기가 움직일 수 있는지 여부, 기본값은 false
    
    // 이 함수가 핵심입니다! 외부(ImoogiTrigger)에서 이 함수를 호출할 겁니다.
    public void StartMoving()
    {
        canMove = true;
    }

    void Update()
    {
        // canMove가 false이면 아무것도 하지 않고 함수를 빠져나갑니다.
        if (!canMove || path == null || path.SegmentCount == 0)
        {
            return;
        }

        // --- (여기는 이전과 동일한 이동 로직입니다) ---
        float totalT = t * path.SegmentCount;
        int segmentIndex = Mathf.FloorToInt(totalT);
        segmentIndex = Mathf.Clamp(segmentIndex, 0, path.SegmentCount - 1);

        float currentSpeed = path.GetSpeed(segmentIndex);

        Vector3[] segmentPoints = path.GetPointsInSegment(segmentIndex);
        float segmentLength = ApproximateSegmentLength(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3]);
        if (segmentLength < 0.01f) segmentLength = 0.01f;

        float deltaT = (currentSpeed / segmentLength) * Time.deltaTime;
        t += deltaT / path.SegmentCount;

        if (t >= 1f)
        {
            t -= 1f;
        }

        transform.position = path.GetPoint(t);

        if (lookForward)
        {
            Vector3 direction = (path.GetPoint(t + 0.001f) - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
    
    

    // 베지어 곡선 세그먼트의 길이를 근사치로 계산하는 도우미 함수 (이전과 동일)
    private float ApproximateSegmentLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float length = 0;
        Vector3 lastPoint = p0;
        int steps = 10;
        for (int i = 1; i <= steps; i++)
        {
            float time = i / (float)steps;
            Vector3 currentPoint = Bezier.EvaluateCubic(p0, p1, p2, p3, time);
            length += Vector3.Distance(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }
        return length;
    }
}