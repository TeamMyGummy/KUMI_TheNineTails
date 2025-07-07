using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbilitySystem.Base;

public class UI_MonsterHpBar : MonoBehaviour
{
    public GameObject prfHpBar;
    public GameObject canvas;

    private RectTransform hpBar;
    private Image hpImage;
    private Attribute hp;

    private float damagedTime = 0f; //마지막으로 피격된 시각 기록
    private CanvasGroup canvasGroup;

    void Start()
    {

        // HP 바 인스턴스 생성
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        hpBar.gameObject.SetActive(false); //비활성화

        canvasGroup = hpBar.GetComponent<CanvasGroup>();

        // hp바의 이미지를 찾기 (실제로 줄어드는 이미지)
        hpImage = hpBar.transform.Find("hp_bar").GetComponent<Image>(); 

        var ga = GetComponent<GameplayAttribute>();
        ga.Attributes.TryGetValue("HP", out var hpAttribute);
        //hp스탯 찾아서 담기
        hp = hpAttribute;
        hpImage.fillAmount = hp.CurrentValue / hp.MaxValue; //hp 비율로 이미지 조정됨
        hp.OnValueChanged += OnHpChanged;
    }

    void Update()
    {
        if (hpBar == null) return;

        // 몬스터 키 구하기
        var renderer = GetComponentInChildren<Renderer>();
        float height = renderer.bounds.max.y;

        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, height + 0.2f, transform.position.z));
        hpBar.position = hpBarPos;

        float damageTimer = Time.time - damagedTime;
        if (damageTimer <= 0.7f) //0.7초동안 선명하게 표기
            canvasGroup.alpha = 1f;
        else if (damageTimer <= 1.0f) { //0.7초 이내 재피격당하지 않을 경우 0.3초동안 페이드아웃
            float t = (damageTimer - 0.7f) / 0.3f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t); //페이드아웃
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

        // 피격 시 hp바 띄움
        if (newHp < hp.MaxValue)
        {
            damagedTime = Time.time;
            hpBar.gameObject.SetActive(true);
  
        }
    }

    void OnDestroy()
    {
        //Destroy(hpBar.gameObject);
    }
}
