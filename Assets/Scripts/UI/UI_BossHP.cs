using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using GameAbilitySystem;
using Cysharp.Threading.Tasks;

public class UI_BossHp : MonoBehaviour
{
    [Header("Boss Target")]
    [SerializeField] private GameObject bossObject;

    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI nameText;

    private Attribute _hpAttr;
    private CompositeDisposable _disp = new();

    private async void OnEnable()
    {
        await UniTask.NextFrame();

        if (!BindToBossHp()) return;
        InitHpBar();
    }

    private void OnDisable()
    {
        _disp.Dispose();
        _disp = new CompositeDisposable();
    }

    private bool BindToBossHp()
    {
        if (bossObject == null || fillImage == null)
        {
            Debug.LogWarning("[UI_BossHp] bossObject 또는 fillImage가 비었습니다.");
            return false;
        }

        var asc = bossObject.GetComponent<IAbilitySystem>()?.asc
                ?? bossObject.GetComponentInChildren<IAbilitySystem>()?.asc;
        if (asc == null) return false;

        if (!asc.Attribute.Attributes.TryGetValue("HP", out _hpAttr) || _hpAttr == null)
            return false;

        if (nameText != null)
            nameText.text = bossObject.name;

        // HP 변화 구독
        _hpAttr.CurrentValue
            .Subscribe(cur => UpdateBar(cur, _hpAttr.MaxValue))
            .AddTo(_disp);

        // MaxHP 변화 구독
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
}
