using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FloorText : MonoBehaviour
{
    [SerializeField] private float visibleTime = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Invoke(nameof(StartFadeOut), visibleTime);
    }

    void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
