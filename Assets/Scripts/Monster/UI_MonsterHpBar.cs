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
    private Monster monster;

    private float damagedTime;

    private IEnumerator Start()
    {
        monster = GetComponent<Monster>();

        while (!monster.asc.Attribute.Attributes.ContainsKey("HP"))
            yield return null;

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
        if (monster == null || hpBar == null || Camera.main == null || hp == null) return;

        float offsetY = 1.2f;
        if (TryGetComponent<Collider2D>(out var col))
            offsetY = col.bounds.size.y + 0.3f;

        Vector3 worldPos = transform.position + new Vector3(0, offsetY - 0.5f, 0);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos.y -= 20f;
        hpBar.position = screenPos;

        float t = Time.time - damagedTime;

        if (monster.isAggro)
        {
            hpBar.gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
        }
        else
        {
            if (t <= 0.7f)
            {
                canvasGroup.alpha = 1f;
            }
            else if (t <= 1.0f)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (t - 0.7f) / 0.3f);
            }
            else
            {
                canvasGroup.alpha = 0f;
                hpBar.gameObject.SetActive(false);
            }
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
        if (hpBar != null)
        {
            Destroy(hpBar.gameObject);
        }
    }
}
