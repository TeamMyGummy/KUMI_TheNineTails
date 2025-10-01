using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField]
    private BezierPath path; // 따라갈 경로

    [SerializeField]
    private float speed = 5f; // 이동 속도

    [SerializeField]
    private bool lookForward = true; // 이동 방향을 바라보게 할지 여부
    
    private float t = 0f; // 경로 상의 위치 (0.0 ~ 1.0)

    void Update()
    {
        if (path == null) return;

        // 't' 값을 속도에 따라 증가시킵니다.
        // 경로의 길이를 대략적으로 계산하여 속도를 일정하게 만듭니다. (정확하진 않지만 간단한 방법)
        float pathLength = Vector3.Distance(path.GetPoint(0), path.GetPoint(1));
        t += (speed / pathLength) * Time.deltaTime;

        // t가 1을 넘으면 처음으로 돌아가도록 (루프)
        if (t > 1f)
        {
            t -= 1f;
        }

        // 경로 상의 현재 위치를 계산하여 오브젝트 위치를 업데이트합니다.
        transform.position = path.GetPoint(t);

        // 이동 방향을 바라보도록 오브젝트를 회전시킵니다.
        if (lookForward)
        {
            Vector3 direction = (path.GetPoint(t + 0.01f) - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // 2D 스프라이트가 기본적으로 오른쪽을 보고 있다면 Z축 회전만 필요합니다.
                // 만약 스프라이트가 위쪽을 보고 있다면 'angle - 90f' 로 수정해야 할 수 있습니다.
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
