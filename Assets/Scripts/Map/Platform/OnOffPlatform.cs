using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffPlatform : MonoBehaviour
{
    public float visibleTime = 2f;   // 보이는 시간
    public float hiddenTime = 2f;    // 사라지는 시간

    private SpriteRenderer _rend;
    private Collider2D _collider;

    void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }
    void Start()
    {
        StartCoroutine(TogglePlatform());
    }

    IEnumerator TogglePlatform()
    {
        while (true)
        {
            // 보이게
            _rend.enabled = true;
            _collider.enabled = true;
            yield return new WaitForSeconds(visibleTime);

            // 숨기기
            _rend.enabled = false;
            _collider.enabled = false;
            yield return new WaitForSeconds(hiddenTime);
        }
    }
}
