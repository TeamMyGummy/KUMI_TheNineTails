using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressMove : MonoBehaviour
{
    public float downDistance = 3f;
    public float downSpeed = 20f;
    public float stopTime = 1f;
    public float upDuration = 2f;
    public float waitBeforeDrop = 1f;

    private GameObject _damageTrigger;   // 자식 오브젝트

    private Vector3 _startPos;
    private Vector3 _targetDownPos;

    void Start()
    {
        _damageTrigger = transform.GetChild(0).gameObject;
        
        _startPos = transform.position;
        _targetDownPos = _startPos + Vector3.down * downDistance;

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            // 내려가기 전에 DamageTrigger ON
            if (_damageTrigger != null)
                _damageTrigger.SetActive(true);

            // 빠르게 내려가기
            while (Vector3.Distance(transform.position, _targetDownPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _targetDownPos,
                    downSpeed * Time.deltaTime
                );
                yield return null;
            }

            // 바닥에서 정지
            yield return new WaitForSeconds(stopTime);

            // 올라가기 전에 DamageTrigger OFF
            if (_damageTrigger != null)
                _damageTrigger.SetActive(false);

            // 천천히 올라가기
            float t = 0;
            while (t < upDuration)
            {
                t += Time.deltaTime;
                float ratio = t / upDuration;
                transform.position = Vector3.Lerp(_targetDownPos, _startPos, ratio);
                yield return null;
            }

            // 올라간 후 대기
            yield return new WaitForSeconds(waitBeforeDrop);
        }
    }
}
