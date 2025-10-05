using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections;

public class UI_MonsterHpBar : MonoBehaviour
{
    public GameObject prfHpBar;
    
    private Monster monster;
    private RectTransform hpBar;
    private Image hpImage;
    private CanvasGroup canvasGroup;
    private GameAbilitySystem.Attribute hp;

    private Transform canvasTransform;
    private Camera mainCamera;
    private Collider2D monsterCollider;

    private float lastDamagedTime;
    private readonly CompositeDisposable _disposables = new();

    private IEnumerator Start()
    {
        monster = GetComponent<Monster>();
        mainCamera = Camera.main;
        monsterCollider = GetComponent<Collider2D>(); 
    
        GameObject canvasObj = GameObject.Find("BaseCanvas");
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;
        }
        else
        {
            Debug.LogError("BaseCanvas를 찾을 수 없습니다!");
            yield break;
        }

        while (monster.asc == null || !monster.asc.Attribute.Attributes.ContainsKey("HP"))
        {
            yield return null;
        }
        hp = monster.asc.Attribute.Attributes["HP"];

        InitializeHpBar();
    }

    void LateUpdate()
    {
        if (hpBar == null) return;
        UpdateVisibility();

        if (!hpBar.gameObject.activeInHierarchy)
        {
            return;
        }

        if (hp.CurrentValue.Value <= 0)
        {
            hpBar.gameObject.SetActive(false);
            return;
        }
    
        UpdateHpBarPosition();
    }
  
    private void InitializeHpBar()
    {
        hpBar = Instantiate(prfHpBar, canvasTransform).GetComponent<RectTransform>();
        canvasGroup = hpBar.GetComponent<CanvasGroup>();
        hpImage = hpBar.transform.Find("hp_bar").GetComponent<Image>();

        hpImage.fillAmount = hp.CurrentValue.Value / hp.MaxValue;
        
        hp.CurrentValue.Subscribe(OnHpChanged).AddTo(_disposables);
        hpBar.gameObject.SetActive(false);
    }

    private void UpdateHpBarPosition()
    {
        if (monsterCollider == null)
        {
            Vector3 worldPosition = transform.position + new Vector3(0, 1.2f, 0);
            hpBar.position = mainCamera.WorldToScreenPoint(worldPosition);
        }
        else
        {
            float topY = monsterCollider.bounds.max.y + 0.2f;

            Vector3 worldPosition = new Vector3(transform.position.x, topY + 0.3f, transform.position.z);

            hpBar.position = mainCamera.WorldToScreenPoint(worldPosition);
        }
    }

    private void UpdateVisibility()
    {
        if (monster.isAggro)
        {
            if (!hpBar.gameObject.activeInHierarchy)
            {
                hpBar.gameObject.SetActive(true);
            }
            canvasGroup.alpha = 1f;
            return;
        }

        float timeSinceDamage = Time.time - lastDamagedTime;

        if (timeSinceDamage <= 2.0f)
        {
            canvasGroup.alpha = 1f;
        }
        else if (timeSinceDamage <= 3.0f)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timeSinceDamage - 2.0f) / 1.0f);
        }
        else
        {
            canvasGroup.alpha = 0f;
            if (hpBar.gameObject.activeInHierarchy)
            {
                hpBar.gameObject.SetActive(false);
            }
        }
    }

    void OnHpChanged(float newHp)
    {
        if (hpBar == null) return;
        
        hpImage.fillAmount = newHp / hp.MaxValue;

        if (newHp < hp.MaxValue && newHp > 0)
        {
            lastDamagedTime = Time.time;
            if (!hpBar.gameObject.activeInHierarchy)
            {
                hpBar.gameObject.SetActive(true);
            }
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