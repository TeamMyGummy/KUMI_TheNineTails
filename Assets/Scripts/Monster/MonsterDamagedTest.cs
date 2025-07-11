using System.Collections;
using UnityEngine;
using AbilitySystem.Base;

public class MonsterDamagedTest : MonoBehaviour
{
    public float damagePerTick = 10f;
    public float interval = 3f;

    private GameplayAttribute attribute;
    private Attribute hp;

    private void Start()
    {
        attribute = GetComponent<GameplayAttribute>();

        if (attribute == null || !attribute.Attributes.TryGetValue("HP", out hp))
        {
            Debug.LogWarning($"{gameObject.name}�� HP Attribute�� �����ϴ�.");
            return;
        }

        StartCoroutine(AutoDamageLoop());
    }

    private IEnumerator AutoDamageLoop()
    {
        while (hp.CurrentValue > 0)
        {
            yield return new WaitForSeconds(interval);

            InstantGameplayEffect effect = new("HP", -damagePerTick);
            effect.Apply(attribute);

            if (hp.CurrentValue <= 0)
            {
                Debug.Log($"{gameObject.name} ���, ������Ʈ ������");
                Destroy(gameObject);  // ���� ����
                yield break;
            }
        }
    }
}
