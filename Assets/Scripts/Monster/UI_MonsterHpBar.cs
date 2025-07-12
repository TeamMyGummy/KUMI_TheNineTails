using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections;

public class UI_MonsterHpBar : MonoBehaviour
{
    public GameObject prfHpBar;
    public GameObject canvas;

    private RectTransform hpBar;
    private Image hpImage;
    private GameAbilitySystem.Attribute hp;
    private CanvasGroup canvasGroup;
    private CompositeDisposable _disposables = new();

    private float damagedTime;

    private IEnumerator Start()
    {
        // ASC가 초기화될 때까지 기다림
        var monster = GetComponent<Monster>();
        while (!monster.asc.Attribute.Attributes.ContainsKey("HP"))
        {
            yield return null;
        }

        hp = monster.asc.Attribute.Attributes["HP"];

        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        hpBar.gameObject.SetActive(false);
        canvasGroup = hpBar.GetComponent<CanvasGroup>();
        hpImage = hpBar.transform.Find("hp_bar").GetComponent<Image>();

        hpImage.fillAmount = hp.CurrentValue.Value / hp.MaxValue;
        _disposables.Add(hp.CurrentValue.Subscribe(OnHpChanged));
    }

    void Update()
    {
        if (hpBar == null) return;

        var renderer = GetComponentInChildren<Renderer>();
        float height = renderer.bounds.max.y;
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, height + 0.2f, transform.position.z));
        hpBar.position = hpBarPos;

        float damageTimer = Time.time - damagedTime;
        if (damageTimer <= 0.7f)
            canvasGroup.alpha = 1f;
        else if (damageTimer <= 1.0f)
        {
            float t = (damageTimer - 0.7f) / 0.3f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
        }
        else
        {
            canvasGroup.alpha = 0f;
            hpBar.gameObject.SetActive(false);
        }
    }

    void OnHpChanged(float newHp)
    {
        hpImage.fillAmount = newHp / hp.MaxValue;
        if (newHp < hp.MaxValue)
        {
            damagedTime = Time.time;
            hpBar.gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        _disposables.Dispose();
    }
}
