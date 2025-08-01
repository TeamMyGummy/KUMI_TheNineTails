using System.Collections;
using System.Collections.Generic;
using Managers;
using R3;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UI_PlayerState : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI _hp;
    [SerializeField] private List<Sprite> hpSprites;
    [SerializeField] private Image hpImage;
    [SerializeField] private List<Sprite> skillSprites;
    [SerializeField] private Image skillImage;
    [SerializeField] private Image foxFireGaugeImage;
    [SerializeField] private Sprite[] foxFireGaugeSprites; 
    [SerializeField] private Image foxFireCountImage;
    [SerializeField] private Sprite[] foxFireCountSprites;
    private CompositeDisposable _disposables = new();
    private VM_PlayerState _playerVM;

    void Awake()
    {
        _playerVM = GetComponent<VM_PlayerState>();
    }

    private async void OnEnable()
    {
        await UniTask.NextFrame(); // 한 프레임 뒤로 미룸

        if (_playerVM == null)
        {
            Debug.LogError("[UI] _playerVM is null");
            return;
        }

        if (_playerVM.SkillCount == null)
        {
            Debug.LogError("[UI] SkillCount is null");
            return;
        }

        _playerVM.Hp
            .Subscribe(val => UpdateHp((int)val))
            .AddTo(_disposables);

        _playerVM.SkillCount
            .Take(1)
            .Subscribe(UpdateSkillProfile)
            .AddTo(_disposables);

        _disposables.Add(_playerVM.SkillCount.Subscribe(UpdateSkillProfile));
        _disposables.Add(_playerVM.FoxFireGauge.Subscribe(UpdateFoxFireGauge));
        _disposables.Add(_playerVM.FoxFireCount.Subscribe(UpdateFoxFireCount));

        UpdateFoxFireGauge(_playerVM.FoxFireGauge.CurrentValue);
        UpdateFoxFireCount(_playerVM.FoxFireCount.CurrentValue);
    }


    //추후 해당 코드 변경
    public void UpdateHp(int hp)
    {
        //_hp.text = hp.ToString();
        int hpIndex = Mathf.Clamp(Mathf.FloorToInt(hp), 0, hpSprites.Count - 1);
        hpImage.sprite = hpSprites[hpIndex];
    }

    private void UpdateSkillProfile(int skillCount)
    {
        //Debug.Log($"[SkillUI] SkillCount: {skillCount}, SpriteCount: {skillSprites.Count}");

        if (skillCount >= 0 && skillCount < skillSprites.Count)
        {
            skillImage.sprite = skillSprites[skillCount];
        }
        else
        {
            //Debug.LogWarning($"[SkillUI] 스킬 수 {skillCount}가 skillSprites 범위를 벗어남");
        }
    }

    void UpdateFoxFireGauge(float value)
    {
        int stage = Mathf.Clamp(Mathf.FloorToInt(value), 0, foxFireGaugeSprites.Length - 1);
        foxFireGaugeImage.sprite = foxFireGaugeSprites[stage];
    }

    void UpdateFoxFireCount(int count)
    {
        int clamped = Mathf.Clamp(count, 0, foxFireCountSprites.Length - 1);
        foxFireCountImage.sprite = foxFireCountSprites[clamped];
    }

    void OnDestroy()
    {
        _disposables.Dispose();
    }
}

