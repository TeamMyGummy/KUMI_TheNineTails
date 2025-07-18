using System.Collections;
using UnityEngine;

public class UI_StartDialogue : MonoBehaviour
{
    [SerializeField] private CanvasGroup background; // 뒷화면 서서히 어둡게하는  검은색 image + CanvasGroup
    private GameObject[] uiToHide; // 숨길 UI들 - 오브젝트 태그 UI로 설정
    private PlayerController player;

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

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        float duration = 0.5f;
        float targetAlpha = 0.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            background.alpha = Mathf.Lerp(0f, targetAlpha, time / duration);
            yield return null;
        }

        background.alpha = targetAlpha;
    }

}