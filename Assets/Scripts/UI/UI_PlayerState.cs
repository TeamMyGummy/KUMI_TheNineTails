using System.Collections;
using System.Collections.Generic;
using Managers;
using R3;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerState : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI _hp;
    [SerializeField] private List<Sprite> hpSprites;
    [SerializeField] private Image hpImage;
    [SerializeField] private List<Sprite> skillSprites;
    [SerializeField] private Image skillImage;
    private CompositeDisposable _disposables = new();
    private VM_PlayerState _playerVM;

    void Awake()
    {
        _playerVM = GetComponent<VM_PlayerState>();

    }
    void Start()
    {
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
        Debug.Log($"[SkillUI] SkillCount: {skillCount}, SpriteCount: {skillSprites.Count}");

        if (skillCount >= 0 && skillCount < skillSprites.Count)
        {
            skillImage.sprite = skillSprites[skillCount];
        }
        else
        {
            Debug.LogWarning($"[SkillUI] 스킬 수 {skillCount}가 skillSprites 범위를 벗어남");
        }
    }

    void OnDestroy()
    {
        _disposables.Dispose();
    }
}

