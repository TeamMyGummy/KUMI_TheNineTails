// UI_PlayerState.cs
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

    [Header("Skill")]
    [SerializeField] private List<Sprite> skillSprites;
    [SerializeField] private Image skillImage;

    [Header("패링 게이지 (4단)")]
    [SerializeField] private Image gaugeBaseImage; 
    [SerializeField] private List<Image> foxFireGaugePetals; 
    [SerializeField] private Image gaugeFullImage; 
    
    private List<UI_Glow> _foxFireGlowScripts = new List<UI_Glow>();
    private UI_Glow _fullGlowScript; 
    
    private int _previousGaugeStage = -1; 
    private bool _isFullGaugeFadingOut = false; 

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
        
        // --- HP 슬롯 초기화 ---
        for (int i = 0; i < hpContainer.childCount; i++)
        {
            var child = hpContainer.GetChild(i);
            var image = child.GetComponent<Image>();
            if (image != null) hpImages.Add(image);
        }
        
        // --- FoxFire 구슬 초기화 ---
        for (int i = 0; i < foxFireContainer.childCount; i++)
        {
            var child = foxFireContainer.GetChild(i);
            var image = child.GetComponent<Image>();
            if (image != null) _foxFireImages.Add(image);
        }
        
        _foxFireGlowScripts.Clear();
        foreach (var petalImage in foxFireGaugePetals)
        {
            if (petalImage.TryGetComponent<UI_Glow>(out var glowScript))
            {
                _foxFireGlowScripts.Add(glowScript);
            }
            else Debug.LogWarning($"[UI] {petalImage.gameObject.name}에 UI_Glow 스크립트가 없습니다.", petalImage.gameObject);
        }

        if (gaugeFullImage != null)
        {
            if (!gaugeFullImage.TryGetComponent<UI_Glow>(out _fullGlowScript))
            {
                Debug.LogWarning($"[UI] {gaugeFullImage.gameObject.name}에 UI_Glow 스크립트가 없습니다.", gaugeFullImage.gameObject);
            }
        }
        
        _isFullGaugeFadingOut = false;
        SetGaugeVisuals(0, true); 
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
        _playerVM.Hp.Subscribe(current => UpdateHp(current, _playerVM.MaxHp)).AddTo(_disposables);
        // 스킬 아이콘
        _playerVM.SkillCount.Subscribe(UpdateSkillProfile).AddTo(_disposables);

        // 여우불 (게이지 및 카운트)
        _disposables.Add(
            _playerVM.FoxFireCount
                .CombineLatest(_playerVM.MaxFoxFireCountRP, _playerVM.FoxFireGauge,
                        (count, max, gauge) => (count, max, gauge))
                .Subscribe(t =>
                {
                    RefreshFoxFire(t.count, t.max);
                    UpdateFoxFireGauge(t.count, t.max, t.gauge);
                })
        );

        // 혼불
        if (HonbulCountText != null)
        {
            _playerVM.HonbulCount.Subscribe(n => { HonbulCountText.text = $"X {n}"; }).AddTo(_disposables);
        }

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
        while (hpImages.Count < maxHp)
        {
            var go = Instantiate(additionalHpPrefab, hpContainer);
            var img = go.GetComponent<Image>();
            hpImages.Add(img);
        }
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
        }
    }

    // ------ 스킬 아이콘 ------
    private void UpdateSkillProfile(int skillCount)
    {
        if (skillCount >= 0 && skillCount < skillSprites.Count)
            skillImage.sprite = skillSprites[skillCount];
    }

    // ------ 패링 게이지 ------
    private void UpdateFoxFireGauge(int count, int max, float value)
    {
        if (_isFullGaugeFadingOut) return;

        int currentStage; 
        
        if (count >= max)
        {
            currentStage = 0;
        }
        else
        {
            currentStage = Mathf.FloorToInt(value); 
            currentStage = Mathf.Clamp(currentStage, 0, 4); 
        }

        if (currentStage == 4 && _previousGaugeStage != 4)
        {
            SetGaugeVisuals(4);
            if (_fullGlowScript != null)
            {
                _fullGlowScript.PlayGlow();
            }
        }
        else if (currentStage == 0 && _previousGaugeStage == 4)
        {
            StartFadeOutFullGauge().Forget();
        }
        else if (currentStage < 4)
        {
            SetGaugeVisuals(currentStage);
            if (currentStage > _previousGaugeStage)
            {
                int petalIndex = currentStage - 1;
                if (petalIndex >= 0 && petalIndex < _foxFireGlowScripts.Count)
                {
                    _foxFireGlowScripts[petalIndex].PlayGlow();
                }
            }
        }

        _previousGaugeStage = currentStage;
    }

    private async UniTaskVoid StartFadeOutFullGauge()
    {
        _isFullGaugeFadingOut = true;

        float duration = 0.5f;
        if (_fullGlowScript != null)
        {
            duration = _fullGlowScript.glowDuration; 
        }
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));

        SetGaugeVisuals(0); 
        _previousGaugeStage = 0;
        _isFullGaugeFadingOut = false;
    }
    
    private void SetGaugeVisuals(int stage, bool force = false)
    {
        for (int i = 0; i < foxFireGaugePetals.Count; i++)
        {
            if (foxFireGaugePetals[i] == null) continue;
            Image petal = foxFireGaugePetals[i];
            
            bool isVisible = (stage == 4) ? false : (i < stage); 
            Color c = petal.color;
            petal.color = new Color(c.r, c.g, c.b, isVisible ? 1f : 0f);
        }
        if (gaugeFullImage != null)
        {
            bool isFull = (stage == 4);
            Color c = gaugeFullImage.color;
            gaugeFullImage.color = new Color(c.r, c.g, c.b, isFull ? 1f : 0f);
        }
        if (gaugeBaseImage != null)
        {
            bool isBaseVisible = (stage != 4); 
            if (force) isBaseVisible = true; 

            Color c = gaugeBaseImage.color;
            gaugeBaseImage.color = new Color(c.r, c.g, c.b, isBaseVisible ? 1f : 0f);
        }
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