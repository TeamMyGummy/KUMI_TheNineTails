using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using GameAbilitySystem;
using Cysharp.Threading.Tasks;

public class UI_BossHp : MonoBehaviour
{
    [SerializeField] private GameObject bossObject;        
    [SerializeField] private Image fillImage;              
    [SerializeField] private TextMeshProUGUI nameText;

    private Attribute _hpAttr;        
    private CompositeDisposable _disp = new();

    private async void OnEnable()
    {
        await UniTask.NextFrame();

        if (bossObject == null || fillImage == null)
            return;

        var asc = bossObject.GetComponent<IAbilitySystem>()?.asc
                ?? bossObject.GetComponentInChildren<IAbilitySystem>()?.asc;
        if (asc == null)
            return;

        if (!asc.Attribute.Attributes.TryGetValue("HP", out _hpAttr) || _hpAttr == null)
            return;

            nameText.text = bossObject.name;

        UpdateBar(_hpAttr.CurrentValue.Value, _hpAttr.MaxValue);

        _hpAttr.CurrentValue
            .Subscribe(cur => UpdateBar(cur, _hpAttr.MaxValue))
            .AddTo(_disp);

        _hpAttr.MaxValueRP
            .Subscribe(_ => UpdateBar(_hpAttr.CurrentValue.Value, _hpAttr.MaxValue))
            .AddTo(_disp);
    }

    private void OnDisable()
    {
        _disp.Dispose();
        _disp = new CompositeDisposable();
    }

    private void UpdateBar(float cur, float max)
    {
        if (fillImage == null) return;
        float ratio = (max > 0f) ? cur / max : 0f;
        fillImage.fillAmount = Mathf.Clamp01(ratio);
    }
}
