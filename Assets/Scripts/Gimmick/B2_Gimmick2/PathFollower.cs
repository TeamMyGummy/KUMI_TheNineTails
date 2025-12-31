using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private BezierPath path;
    [SerializeField] private bool lookForward = true;
    [SerializeField] private GameObject Imugi;

    private float t = 0f;
    private bool canMove = false;
    
    
    private Animator animator; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        if (animator != null)
        {
            animator.SetTrigger("WakeUp");
            SoundManager.Instance.PlaySFX(SFXName.이무기등장);
        }

        StartCoroutine(WaitTillAnimFinish());
    }

    private void Start()
    {
        SoundManager.Instance.StopBGM();
    }

    public void EndMove()
    {
        t = 1f;
        transform.position = path.GetPoint(t);
        if (animator != null)
        {
            animator.SetTrigger("EndReached");
        }

        StartCoroutine(Wait());
        Debug.Log("EndMove");
    }
    

    void Update()
    {
        if (!canMove || path == null || path.SegmentCount == 0)
        {
            return;
        }

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
            // 이동 멈춤
            canMove = false;
            
            // 위치를 경로의 끝으로 고정
            t = 1f;
            transform.position = path.GetPoint(t);

            if (animator != null)
            {
                animator.SetTrigger("EndReached");
            }

            return;
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

    public void Attack1()
    {
        SoundManager.Instance.PlaySFX(SFXName.이무기공격1);
    }

    public void Attack2()
    {
        SoundManager.Instance.PlaySFX(SFXName.이무기공격2);
    }

    public void Attack3()
    {
        SoundManager.Instance.PlaySFX(SFXName.이무기공격3);
    }

    private IEnumerator WaitTillAnimFinish()
    {
        yield return new WaitForSeconds(4f);
        canMove = true;
    }
    
    void MoveLeftRelative(float distance, float duration)
    {
        transform.DOMoveX(transform.position.x - distance, duration)
            .SetEase(Ease.OutCubic);
    }
    
    private IEnumerator Wait(){
        yield return new WaitForSeconds(9.5f);
        MoveLeftRelative(40f, 20f);
        
    }
}