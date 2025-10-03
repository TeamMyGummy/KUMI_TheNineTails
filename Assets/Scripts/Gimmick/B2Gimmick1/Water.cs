using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float risingSpeed = 1.5f;
    private bool isRising = false;

    private void Update()
    {
        if (isRising)
        {
            transform.Translate(Vector3.up * risingSpeed * Time.deltaTime);
        }
    }

    public void StartRise(float duration)
    {
        StartCoroutine(RiseCoroutine(duration));
    }

    public void StopRise(float duration)
    {
        StartCoroutine(StopCoroutine(duration));
    }

    private IEnumerator RiseCoroutine(float duration)
    {
        isRising = true;
        yield return new WaitForSeconds(duration);
        isRising = false;
    }

    private IEnumerator StopCoroutine(float duration)
    {
        isRising = false;
        yield return new WaitForSeconds(duration);
        isRising = true;
    }
}
