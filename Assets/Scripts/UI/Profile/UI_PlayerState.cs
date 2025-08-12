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
    [SerializeField] private Transform hpContainer;
    private readonly List<Image> _hpImages = new();

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
    }

    private async void OnEnable()
    {
        await UniTask.NextFrame();

        if (_playerVM == null)
        {
            Debug.LogError("[UI] _playerVM is null");
            return;
        }

        // HP 
        _playerVM.Hp
            .Subscribe(current => UpdateHp(current, _playerVM.MaxHp))
            .AddTo(_disposables);

        // 여우불 게이지
        _playerVM.FoxFireGauge
            .Subscribe(UpdateFoxFireGauge)
            .AddTo(_disposables);

        // 스킬 아이콘 - 초기 1회만
        _playerVM.SkillCount
            .Take(1)
            .Subscribe(UpdateSkillProfile)
            .AddTo(_disposables);

        // 여우불 개수 - Max 값이 변할 때만 전체 슬롯 갱신
        _playerVM.MaxFoxFireCountRP
            .DistinctUntilChanged()
            .Subscribe(max =>
            {
                RefreshFoxFire(_playerVM.FoxFireCount.CurrentValue, _playerVM.MaxFoxFireCountRP.CurrentValue);
            })
            .AddTo(_disposables);

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
        UpdateFoxFireGauge(_playerVM.FoxFireGauge.CurrentValue);
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
        while (_hpImages.Count < maxHp)
        {
            var go = new GameObject("Hp");
            var img = go.AddComponent<Image>();
            img.transform.SetParent(hpContainer, false);
            img.sprite = emptyHpSprite;
            _hpImages.Add(img);
        }

        // 표시/스프라이트 갱신
        for (int i = 0; i < _hpImages.Count; i++)
        {
            bool active = i < maxHp;
            _hpImages[i].gameObject.SetActive(active);
            if (active)
                _hpImages[i].sprite = (i < curHp) ? filledHpSprite : emptyHpSprite;
        }
    }

    // ------ 스킬 아이콘 ------
    private void UpdateSkillProfile(int skillCount)
    {
        if (skillCount >= 0 && skillCount < skillSprites.Count)
            skillImage.sprite = skillSprites[skillCount];
    }

    // ------ 여우불 게이지 ------
    private void UpdateFoxFireGauge(float value)
    {
        // 최대 개수인지 체크
        if (_playerVM.FoxFireCount.CurrentValue >= _playerVM.MaxFoxFireCountRP.CurrentValue)
        {
            foxFireGaugeImage.sprite = foxFireGaugeSprites[foxFireGaugeSprites.Length - 1]; // Full 상태
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

        // 슬롯 생성
        while (_foxFireImages.Count < mx)
        {
            var go = new GameObject("FoxFire");
            var img = go.AddComponent<Image>();
            img.transform.SetParent(foxFireContainer, false);
            img.sprite = foxFireEmpty;
            _foxFireImages.Add(img);
        }

        // 표시/스프라이트 갱신
        for (int i = 0; i < _foxFireImages.Count; i++)
        {
            bool active = i < mx;
            _foxFireImages[i].gameObject.SetActive(active);
            if (active)
                _foxFireImages[i].sprite = (i < cur) ? foxFireFilled : foxFireEmpty;
        }
    }
}
