using System;
using System.Resources;
using Cysharp.Threading.Tasks;
using GameAbilitySystem;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve; //날아갈 때 x축의 위치 곡선
    [SerializeField] private float duration = 0.5f; //날아가는 시간
    [SerializeField] private float maxDistance = 3f; //날아가는 거리
    [SerializeField] private Vector3 offset = Vector3.zero; //히트박스 최초 생성 위치 offset / x값에 바라보고 있는 방향 반영됨
    private bool isEnded = false;
    private Vector2 _direction;

    public void InitProjectile(GameObject actor, Vector2 direction)
    {
        _direction = direction;
        offset.x *= direction.x;
        gameObject.Move(actor.transform.position + offset);
    }
    
    void OnEnable()
    {
        FireProjectile().Forget();
    }

    private async UniTaskVoid FireProjectile()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        while (timer < duration && !isEnded)
        {
            float xPos = curve.Evaluate(Mathf.Clamp01(timer / duration));
            transform.position = new Vector3(startPos.x + xPos * maxDistance * _direction.x, startPos.y,
                startPos.z);
            await UniTask.Yield(PlayerLoopTiming.Update);
            timer += Time.deltaTime; 
        }
        
        ResourcesManager.Instance.Destroy(gameObject);
        //todo. 터지는 애니메이션 후 Destroy
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        isEnded = true;
    }

    private void OnDisable()
    {
        isEnded = false;
    }
}
