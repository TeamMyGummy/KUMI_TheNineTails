using UnityEngine;

public class ColliderTriggerHandler : MonoBehaviour
{
    [SerializeField] private FSMController fsmController;
    [SerializeField] private bool successOnTrigger = true; // 접촉 시 성공인지 실패인지
    [SerializeField] private string[] targetTags; // 특정 태그만 감지
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 태그 체크 (설정되어 있다면)
        if (targetTags.Length > 0)
        {
            bool tagFound = false;
            foreach (string tag in targetTags)
            {
                if (other.CompareTag(tag))
                {
                    tagFound = true;
                    break;
                }
            }
            if (!tagFound) return;
        }
        
        var currentAction = fsmController.GetCurrentActionNode();
        if (currentAction != null && currentAction.IsRunning())
        {
            Debug.Log($"[BossAI ColliderTriggerHandler] 조건이 발생해 노드 중단: 다음 노드 분기에 리턴된 값{successOnTrigger}");
            fsmController.CancelCurrentAction(successOnTrigger);
        }
    }
}