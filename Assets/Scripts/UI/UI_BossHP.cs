using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using GameAbilitySystem;
using Cysharp.Threading.Tasks;
using System;
using GASAttribute = GameAbilitySystem.Attribute;

public class UI_BossHp : MonoBehaviour
{
    [Header("Boss Target")]
    [SerializeField] private GameObject bossObject;

    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject profileUI;

    private GASAttribute _hpAttr;
    private CompositeDisposable _disp = new();

    private async void OnEnable()
    {
        if (hpBar != null) hpBar.SetActive(false);
        if (profileUI != null) profileUI.SetActive(false);

        await Cysharp.Threading.Tasks.UniTask.NextFrame();

        if (!BindToBossHp()) return;
        InitHpBar();

        await Delay();
    }

    private void OnDisable()
    {
        _disp.Dispose();
        _disp = new CompositeDisposable();
    }

    private bool BindToBossHp()
    {
        var asc = bossObject.GetComponent<IAbilitySystem>()?.asc
                ?? bossObject.GetComponentInChildren<IAbilitySystem>()?.asc;
        if (asc == null) return false;

        if (!asc.Attribute.Attributes.TryGetValue("HP", out _hpAttr) || _hpAttr == null)
            return false;

        if (nameText != null)
            nameText.text = bossObject.name;

        _hpAttr.CurrentValue
            .Subscribe(cur => UpdateBar(cur, _hpAttr.MaxValue))
            .AddTo(_disp);

        _hpAttr.MaxValueRP
            .Subscribe(_ => UpdateBar(_hpAttr.CurrentValue.Value, _hpAttr.MaxValue))
            .AddTo(_disp);

        return true;
    }

    private void InitHpBar()
    {
        UpdateBar(_hpAttr.CurrentValue.Value, _hpAttr.MaxValue);
    }

    private void UpdateBar(float cur, float max)
    {
        if (fillImage == null) return;
        float ratio = (max > 0f) ? cur / max : 0f;
        fillImage.fillAmount = Mathf.Clamp01(ratio);
    }

    private async UniTask Delay() //임시
    {
        float waitTime = 1.7f;
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));

        if (hpBar != null) hpBar.SetActive(true);
        if (profileUI != null) profileUI.SetActive(true);
    }
}
