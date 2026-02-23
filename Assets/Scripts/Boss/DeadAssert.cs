using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadAssert : MonoBehaviour
{
    public SpineAnimationHandler SpineAnimationHandler;
    public BTController controller;
    public BTContext context;
    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isDead == false)
        {
            //HP 및 BT 종료 여부 체크 후 만약에 HP 0인데 이무기 안 죽으면 걍제로 코드에서 BT 종료시키고 종료 수행

            if(context.ASC.Attributes["HP"].Value <= 0 && controller.isRunning)
            {
                isDead = true;
                controller.StopAI();
                SpineAnimationHandler.SetAnimation("death");
                SoundManager.Instance.PlaySFX(SFXName.이무기_보스_사망);
                StartCoroutine(MoveToCameraOffset(0f, -3f, 5f));
            }
        }
    }

    private const float StopThreshold = 0.01f; // 도착 판정 임계값 (더 정밀하게 조정)

    /// <summary>
    /// 카메라 위치 기준(CameraOffset)으로 특정 좌표까지 이동합니다.
    /// </summary>
    /// <param name="offsetX">카메라 중심으로부터의 X 오프셋</param>
    /// <param name="offsetY">카메라 중심으로부터의 Y 오프셋</param>
    /// <param name="speed">이동 속도 (음수일 경우 즉시 이동)</param>
    public IEnumerator MoveToCameraOffset(float offsetX, float offsetY, float speed)
    {
        // 1. 목적지 계산 (기존 CameraOffset 로직 반영)
        // Z값은 현재 오브젝트의 Z를 유지하거나 카메라의 Z를 따를 수 있습니다.
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 targetDest = new Vector3(cameraPos.x + offsetX, cameraPos.y + offsetY, transform.position.z);

        // 2. 속도가 음수면 즉시 이동 후 종료
        if (speed < 0f)
        {
            transform.position = targetDest;
            yield break;
        }

        // 3. 루프를 돌며 이동 처리
        while (true)
        {
            Vector3 currentPos = transform.position;
            Vector3 vectorToDest = targetDest - currentPos;
            float distanceToDest = vectorToDest.magnitude;

            // 도착 확인
            if (distanceToDest <= StopThreshold)
            {
                transform.position = targetDest;
                yield break;
            }

            // 프레임당 이동 거리 계산
            float frameMoveDistance = speed * Time.deltaTime;

            // 오버슈팅 방지 및 이동
            if (frameMoveDistance >= distanceToDest)
            {
                transform.position = targetDest;
                yield break;
            }
            else
            {
                transform.position += vectorToDest.normalized * frameMoveDistance;
            }

            // 다음 프레임까지 대기
            yield return null;
        }
    }
}
