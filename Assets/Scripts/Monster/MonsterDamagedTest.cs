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
            Debug.LogWarning($"{gameObject.name}에 HP Attribute가 없습니다.");
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
                Debug.Log($"{gameObject.name} 사망, 오브젝트 삭제됨");
                Destroy(gameObject);  // 몬스터 제거
                yield break;
            }
        }
    }
}
