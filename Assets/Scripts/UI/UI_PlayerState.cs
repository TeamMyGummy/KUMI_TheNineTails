using System.Collections.Generic;
using R3;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class UI_PlayerState : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Sprite filledHpSprite;
    [SerializeField] private Sprite emptyHpSprite;
    [SerializeField] private GameObject additionalHpPrefab;
    [SerializeField] private Transform hpContainer;
    private List<Image> hpImages = new List<Image>();
    //private readonly List<Image> _hpImages = new();

    [Header("Skill")]
    [SerializeField] private List<Sprite> skillSprites;
    [SerializeField] private Image skillImage;

    [Header("FoxFire Gauge")]
    [SerializeField] private Image foxFireGaugeImage;
    [SerializeField] private Sprite[] foxFireGaugeSprites;

    [Header("FoxFire Count")]
    [SerializeField] private Transform foxFireContainer;
    [SerializeField] private Sprite foxFireFilled;
    [SerializeField] private Sprite foxFireEmpty;
    private readonly List<Image> _foxFireImages = new();

    [Header("Honbul Count")]
    [SerializeField] private TextMeshProUGUI HonbulCountText;
    private CompositeDisposable _disposables = new();
    private VM_PlayerState _playerVM;

    void Awake()
    {
        _playerVM = GetComponent<VM_PlayerState>();
        for (int i = 0; i < hpContainer.childCount; i++)
        {
            var child = hpContainer.GetChild(i);
            var image = child.GetComponent<Image>();
            if (image != null)
            {
                hpImages.Add(image);
            }
        }
        for (int i = 0; i < foxFireContainer.childCount; i++)
        {
            var child = foxFireContainer.GetChild(i);
            var image = child.GetComponent<Image>();
            if (image != null)
            {
                _foxFireImages.Add(image);
            }
        }
    }

    private async void OnEnable()
    {
        await UniTask.NextFrame(); // VM 초기화 한 프레임 뒤에 바인딩

        if (_playerVM == null)
        {
            Debug.LogError("[UI] _playerVM is null");
            return;
        }

        // HP 
        _playerVM.Hp
            .Subscribe(current => UpdateHp(current, _playerVM.MaxHp))
            .AddTo(_disposables);

        // 스킬 아이콘
        _playerVM.SkillCount
            .Subscribe(UpdateSkillProfile)
            .AddTo(_disposables);

        // 여우불
        _disposables.Add(
            _playerVM.FoxFireCount
                .CombineLatest(_playerVM.MaxFoxFireCountRP, _playerVM.FoxFireGauge,
                            (count, max, gauge) => (count, max, gauge))
                .Subscribe(t =>
                {
                    // 슬롯 (개수/최대)
                    RefreshFoxFire(t.count, t.max);

                    // 게이지 (단계 계산)
                    UpdateFoxFireGauge(t.count, t.max, t.gauge);
                })
        );

        // 혼불
        if (HonbulCountText != null)
        {
            _playerVM.HonbulCount
                .Subscribe(n =>
                {
                    HonbulCountText.text = $"X {n}";
                })
                .AddTo(_disposables);
        }

        // ======== 초기 렌더 ========
        UpdateFoxFireGauge(
            _playerVM.FoxFireCount.CurrentValue,
            _playerVM.MaxFoxFireCountRP.CurrentValue,
            _playerVM.FoxFireGauge.CurrentValue
        );
        UpdateHp(_playerVM.Hp.CurrentValue, _playerVM.MaxHp);
    }

    private void OnDisable()
    {
        _disposables.Dispose();
        _disposables = new CompositeDisposable();
    }

    // ------ HP ------
    private void UpdateHp(float current, float max)
    {
        int maxHp = Mathf.FloorToInt(max);
        int curHp = Mathf.FloorToInt(current);

        // 부족하면 슬롯 생성
        while (hpImages.Count < maxHp)
        {
            var go = Instantiate(additionalHpPrefab, hpContainer);
            var img = go.GetComponent<Image>();
            hpImages.Add(img);

            /*
            var go = new GameObject("Hp");
            var img = go.AddComponent<Image>();
            img.transform.SetParent(hpContainer, false);
            img.sprite = emptyHpSprite;
            _hpImages.Add(img);
            */
        }

        // 슬롯 표시/색상 갱신
        for (int i = 0; i < hpImages.Count; i++)
        {
            bool shouldShow = i < maxHp;
            hpImages[i].gameObject.SetActive(shouldShow);
            
            if (shouldShow)
            {
                bool isFilled = i < curHp;
                Color currentColor = hpImages[i].color;
                hpImages[i].color = new Color(currentColor.r, currentColor.g, currentColor.b, isFilled ? 1f : 0f);
            }
            /*
            hpImages[i].gameObject.SetActive(active);
            if (active)
                hpImages[i].sprite = (i < curHp) ? filledHpSprite : emptyHpSprite;
            */
        }
    }

    // ------ 스킬 아이콘 ------
    private void UpdateSkillProfile(int skillCount)
    {
        if (skillCount >= 0 && skillCount < skillSprites.Count)
            skillImage.sprite = skillSprites[skillCount];
    }

    // ------ 여우불 게이지 ------
    private void UpdateFoxFireGauge(int count, int max, float value)
    {
        if (count >= max)
        {
            foxFireGaugeImage.sprite = foxFireGaugeSprites[foxFireGaugeSprites.Length - 1];
            return;
        }

        int stage = Mathf.Clamp(Mathf.FloorToInt(value), 0, foxFireGaugeSprites.Length - 1);
        foxFireGaugeImage.sprite = foxFireGaugeSprites[stage];
    }

    // ------ 여우불 개수 ------
    private void RefreshFoxFire(int current, int max)
    {
        int cur = Mathf.Max(0, current);
        int mx = Mathf.Max(0, max);

        for (int i = 0; i < _foxFireImages.Count; i++)
        {
            _foxFireImages[i].gameObject.SetActive(true);
            _foxFireImages[i].sprite = (i < cur) ? foxFireFilled : foxFireEmpty;
        }
    }
}
