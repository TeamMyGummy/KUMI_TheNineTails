using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{

    private Image _targetImage;
    private float _fadeDuration = 0.5f;

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
    /// 알파값 커짐
    /// </summary>

    public void FadeInScreen()
    {
        if (_targetImage != null)
        {
            StartCoroutine(FadeToAlpha(0.5f));
        }
    }
	
	/// <summary>
    /// 알파값 0으로 서서히 변함
    /// </summary>
    public void FadeOutScreen()
    {
        if (_targetImage != null)
        {
            StartCoroutine(FadeToAlpha(0f));
        }
    }

    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color color = _targetImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
            _targetImage.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }
        
        _targetImage.color = new Color(color.r, color.g, color.b, targetAlpha);
    }
}
