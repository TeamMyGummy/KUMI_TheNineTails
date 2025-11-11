// UI_MonsterHpBar.cs (Screen Space - Camera 포지셔닝 수정된 전체 코드)
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
    private RectTransform canvasRectTransform; // (신규) 위치 계산에 필요한 캔버스의 RectTransform
    private Camera mainCamera;
    private Camera canvasCamera; // (신규) UI를 렌더링하는 카메라 (UI_Camera)
    private Collider2D monsterCollider;

    private float lastDamagedTime;
    private readonly CompositeDisposable _disposables = new();

    private IEnumerator Start()
    {
        monster = GetComponent<Monster>();
        mainCamera = Camera.main;
        monsterCollider = GetComponent<Collider2D>(); 
    
        // ▼▼▼▼▼▼ Tag로 캔버스를 찾고, RectTransform과 Camera도 캐시 ▼▼▼▼▼▼
        GameObject canvasObj = GameObject.FindWithTag("BaseCanvas");
        
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;
            // (신규) 위치 계산을 위해 RectTransform을 저장
            canvasRectTransform = canvasObj.GetComponent<RectTransform>(); 
            // (신규) 캔버스를 렌더링하는 카메라(UI_Camera)를 저장
            canvasCamera = canvasObj.GetComponent<Canvas>().worldCamera; 
        }
        else
        {
            Debug.LogError($"[UI_MonsterHpBar] 'BaseCanvasTag' 태그를 가진 캔버스를 찾을 수 없습니다! " +
                           $"'BaseCanvas' 오브젝트에 태그가 붙어있는지 확인하세요.", this.gameObject);
            yield break;
        }
        
        if (canvasCamera == null)
        {
             Debug.LogError($"[UI_MonsterHpBar] 'BaseCanvas'에 Render Camera가 할당되지 않았습니다! " +
                           $"'BaseCanvas'의 Render Mode를 Screen Space - Camera로 설정하고 UI_Camera를 할당하세요.", this.gameObject);
             yield break;
        }
        // ▲▲▲▲▲▲ 수정 끝 ▲▲▲▲▲▲

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
    
        // (수정) 새 로직이 적용된 함수 호출
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

    // ▼▼▼▼▼▼ (핵심 수정) 포지셔닝 로직 전체 변경 ▼▼▼▼▼▼
    private void UpdateHpBarPosition()
    {
        if (mainCamera == null) return;

        // 1. 몬스터의 월드 좌표 가져오기
        Vector3 worldPosition;
        if (monsterCollider == null)
        {
            worldPosition = transform.position + new Vector3(0, 1.2f, 0);
        }
        else
        {
            float topY = monsterCollider.bounds.max.y + 0.2f;
            worldPosition = new Vector3(transform.position.x, topY + 0.3f, transform.position.z);
        }

        // 2. 월드 좌표 -> 스크린 좌표로 변환 (by MainCamera)
        Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);

        // 3. 스크린 좌표 -> 캔버스의 로컬 좌표로 변환 (by UI_Camera)
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, // 캔버스의 RectTransform
            screenPoint,         // 변환할 스크린 좌표
            canvasCamera,        // 캔버스를 렌더링하는 카메라 (UI_Camera)
            out localPoint       // 변환된 캔버스 로컬 좌표
        );

        // 4. HP바의 위치(anchoredPosition)를 캔버스 로컬 좌표로 설정
        hpBar.anchoredPosition = localPoint;
    }
    // ▲▲▲▲▲▲ 수정 끝 ▲▲▲▲▲▲

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