using System.Collections;
using UnityEngine;

public class UI_Dialogue : MonoBehaviour
{
    //[SerializeField] private CanvasGroup background; // 뒷화면 서서히 어둡게하는 검은색 image + CanvasGroup
    private GameObject[] uiToHide; // 숨길 UI들 - 오브젝트 태그 UI로 설정해야함
    private PlayerController player;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        uiToHide = GameObject.FindGameObjectsWithTag("UI");
    }

    public void ApplyDialogueOverlay()
    {
        player?.OnDisableAllInput(); // 인풋 중지

        foreach (var ui in uiToHide)
            ui.SetActive(false);
        
        //background.gameObject.SetActive(true);
        //background.alpha = 0f;

        //if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        //fadeRoutine = StartCoroutine(FadeIn());
    }

    public void RemoveDialogueOverlay()
    {
        player?.OnEnableAllInput(); // 인풋 활성화

        foreach (var ui in uiToHide)
            ui.SetActive(true);

        //StartCoroutine(FadeOut());
    }
	/*
    private IEnumerator FadeIn()
    {
        float time = 0f;
        float duration = 0.5f;
        float startAlpha = 0f;
        float targetAlpha = 0.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            background.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        background.alpha = targetAlpha;
        fadeRoutine = null;
        background.gameObject.SetActive(true);
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        float duration = 0.5f;
        float startAlpha = 0.5f;
        float targetAlpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            background.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        background.alpha = targetAlpha;
        background.gameObject.SetActive(false);

        while (time < duration)
        {
            time += Time.deltaTime;
            background.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        background.alpha = targetAlpha;
        background.gameObject.SetActive(false);
    }
	*/
}