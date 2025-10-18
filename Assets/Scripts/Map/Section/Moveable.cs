    using DG.Tweening;
    using UnityEngine;

    public class Moveable : MonoBehaviour
    {
        public float moveDuration = 1.0f;
    
        // 각 오브젝트가 움직이기 시작하는 시간 간격
        public float delayBetweenChildren = 0.2f;

        private bool isTriggered = false;
        
        //코드... 고쳐야 하긴 하는데...
        public void MoveY(float y)
        {
            if (isTriggered) return;
            float currentDelay = 0f;
            isTriggered = true;

            // 1. foreach 반복문은 기본적으로 하이러라키 순서대로 자식들을 순회합니다.
            // 따라서 별도의 정렬 로직이 필요 없습니다.
            foreach (Transform child in transform)
            {
                // 2. 목표 Y 위치를 현재 위치 + 상대 위치로 계산합니다.
                float targetY = child.position.y + y;

                // 계산된 목표 위치와 지연 시간을 적용하여 애니메이션 실행
                child.DOMoveY(targetY, moveDuration).SetDelay(currentDelay);
            
                // 다음 오브젝트를 위한 지연 시간 누적
                currentDelay += delayBetweenChildren;
            }
        }
    }
