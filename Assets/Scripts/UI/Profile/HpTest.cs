using GameAbilitySystem;
using UnityEngine;
using Util;

public class HpTestButton : MonoBehaviour
{
    public void OnClick_Hit()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out AbilitySystem asc);
        if (asc != null)
        {
            asc.Attribute.Attributes["HP"].Modify(-1, ModOperation.Additive);
            Debug.Log($"[HP] -1 → {asc.Attribute.Attributes["HP"].CurrentValue.Value}");
        }
    }

    public void OnClick_Heal()
    {
        DomainFactory.Instance.GetDomain(DomainKey.Player, out AbilitySystem asc);
        if (asc != null)
        {
            asc.Attribute.Attributes["HP"].Modify(+1, ModOperation.Additive);
            Debug.Log($"[HP] +1 → {asc.Attribute.Attributes["HP"].CurrentValue.Value}");
        }
    }
}
