using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{

    private Image _targetImage;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
    }

    /// <summary>
    /// 화면 활성화
    /// </summary>
    public void ShowScreen()
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    /// <summary>
    /// 화면 비활성화
    /// </summary>

    public void HideScreen()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 알파값 조절
    /// </summary>

    public void FadeScreen(float duration, float targetAlpha)
    {
        if (_targetImage != null)
        {
            ShowScreen();
            float startAlpha = _targetImage.color.a;
            StartCoroutine(FadeToAlpha(startAlpha, targetAlpha, duration));
        }
    }
    

    private IEnumerator FadeToAlpha(float startAlpha, float targetAlpha, float duration)
    {
        Color color = _targetImage.color;
        float start = startAlpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(start, targetAlpha, time / duration);
            _targetImage.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }
        
        _targetImage.color = new Color(color.r, color.g, color.b, targetAlpha);
    }
}
