using System.Collections;
using GameAbilitySystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingMessageTrigger : MonoBehaviour
{
    [Header("Object UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject beforeUseImage;
    [SerializeField] private GameObject afterUseImage;

    [Header("Fade UI")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private GameObject endingTextObject;

    [Header("Timing")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float endingDuration = 3f;

    private bool _playerInRange = false;
    private bool _isUsed = false;

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);

        if (beforeUseImage != null)
            beforeUseImage.SetActive(true);

        if (afterUseImage != null)
            afterUseImage.SetActive(false);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (endingTextObject != null)
            endingTextObject.SetActive(false);
    }

    private void Update()
    {
        if (_playerInRange && !_isUsed)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RefillHp();
            }
        }
    }

    public void RefillHp()
    {
        if (!_playerInRange || _isUsed)
            return;

        beforeUseImage.SetActive(false);
        afterUseImage.SetActive(true);

        AbilitySystem asc;
        DomainFactory.Instance.GetDomain(DomainKey.Player, out asc);
        GameplayAttribute att = asc.Attribute;

        var effect = new HpRefillEffect("HP");
        effect.Apply(att);

        _isUsed = true;
        interactionUI.SetActive(false);

        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(delayBeforeFade);
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        endingTextObject.SetActive(true);
        yield return new WaitForSeconds(endingDuration);
        SceneManager.LoadScene("Start");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isUsed) return;

        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            interactionUI.SetActive(false);
        }
    }
}