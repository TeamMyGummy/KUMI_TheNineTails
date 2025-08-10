using UnityEngine;
using Cysharp.Threading.Tasks;

// IProjectile, ResourcesManager 등 기존 참조는 그대로 사용한다고 가정합니다.
public class Projectile : MonoBehaviour //, IProjectile
{
    [SerializeField] private AnimationCurve curve; //날아가는 동안의 속도/이동 비율 곡선
    [SerializeField] private float duration = 0.5f; //날아가는 시간
    [SerializeField] private float maxDistance = 3f; //날아가는 최대 거리
    [SerializeField] private Vector3 offset = Vector3.zero; //히트박스 최초 생성 위치 offset
    
    private bool isEnded = false;
    private Vector2 _direction;

    //Instantiate이 선행되어야 함
    public void FireProjectile(GameObject actor, Vector2 direction)
    {
        _direction = direction.normalized;
        transform.position = actor.transform.position + offset;
        
        // 발사체의 방향 = 실제 조준 방향
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);

        Fire().Forget();
    }

    public void SetOffset(Vector3 changeOffset)
    {
        offset = changeOffset;
    }

    private async UniTaskVoid Fire()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        // 최종 목표 지점 계산
        Vector3 endPos = startPos + (Vector3)_direction * maxDistance;

        while (timer < duration && !isEnded)
        {
            float timeRatio = Mathf.Clamp01(timer / duration);
            float progress = curve.Evaluate(timeRatio);

            transform.position = Vector3.Lerp(startPos, endPos, progress);

            await UniTask.Yield(PlayerLoopTiming.Update);
            timer += Time.deltaTime;
        }

        DestroyProjectile();
    }
    
    private void DestroyProjectile()
    {
        ResourcesManager.Instance.Destroy(gameObject);
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{other.name}과 충돌!");
        isEnded = true;
    }

    private void OnDisable()
    {
        isEnded = false;
    }
}