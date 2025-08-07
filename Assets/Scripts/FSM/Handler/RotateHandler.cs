using UnityEngine;

public class RotateHandler : ActionHandler
{
    [SerializeField] private float speed = 2f;
    private float _angle;
    private RotationType _type;
    private float _currentDest; // 최종 절대 회전각도
    private bool _rotationComplete = false;

    public void SetRotation(float angle, RotationType type, float speed)
    {
        _angle = angle;
        _type = type;
        if (speed != 0f) this.speed = speed;
    }

    public override void OnEnterAction()
    {
        _rotationComplete = false;

        if (_type == RotationType.Absolute)
        {
            _currentDest = _angle;
        }
        else if (_type == RotationType.OffSet)
        {
            _currentDest = transform.eulerAngles.z + _angle;
        }
    }

    public override bool OnExecuteAction()
    {
        if (_rotationComplete) return true;

        float currentAngles = transform.eulerAngles.z;
        float targetAngles = _currentDest;

        // X, Y, Z 축 각각 최단 회전 방향 계산
        float deltaZ = Mathf.DeltaAngle(currentAngles, targetAngles);

        // 회전 속도 계산
        float step = speed * Time.deltaTime;

        // 회전 속도만큼 목표 각도를 초과하지 않게 회전
        float newZ = Mathf.MoveTowardsAngle(currentAngles, targetAngles, step * Mathf.Abs(deltaZ));
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, newZ);

        // 목표 회전 도달 체크 (오차 범위 0.1도)
        if (Mathf.Abs(Mathf.DeltaAngle(newZ, targetAngles)) < 0.1f)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetAngles);
            _rotationComplete = true;
            return true; // Action 완료
        }

        return false; // 아직 회전 중
    }
}
