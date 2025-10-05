using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock1 : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.7f;
    [SerializeField] private float fallDuration = 0.3f;
    private bool _isTriggered = false;
    private Vector3 _originalPos;

    void Start()
    {
        _originalPos = transform.position;
    }

    public void TriggerFall()
    {
        if (!_isTriggered)
            StartCoroutine(FallSequence());
    }

    private IEnumerator FallSequence()
    {
        _isTriggered = true;

        // 1) 흔들림
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Mathf.Sin(Time.time * 50f) * 0.05f;
            transform.position = _originalPos + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2) 낙하
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false; // 물리 낙하 시작
        yield return new WaitForSeconds(fallDuration);
    }
}
