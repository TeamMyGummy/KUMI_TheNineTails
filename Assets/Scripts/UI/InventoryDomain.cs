using System;
using System.Collections.Generic;

namespace Game.Inventory
{
    public enum ItemType
    {
        MaxHp,
        FoxFire,
        Honbul
    }

    [Serializable]
    public class ItemData
    {
        public ItemType Type;
        public int Amount;

        public int Count { get => Amount; set => Amount = value; }
    }

    [Serializable]
    public class InventoryDto
    {
        public List<ItemData> Items = new();
    }

    public class InventoryDomain : BaseDomain<InventoryDto>
    {
        public List<ItemData> Items = new();

        // UI 실시간 갱신용 이벤트
        public event Action OnChanged;

        // ───────── BaseDomain<T> 구현 ─────────
        public override void Init(string path)
        {
        }

        public override void Load(InventoryDto dto)
        {
            Items = dto?.Items ?? new List<ItemData>();
            OnChanged?.Invoke();
        }

        public override InventoryDto Save()
        {
            return new InventoryDto { Items = Items };
        }
        // ─────────────────────────────────────

        public void AddItem(ItemType type, int amount = 1)
        {
            var exist = Items.Find(i => i.Type == type);
            if (exist != null) exist.Amount += amount;
            else Items.Add(new ItemData { Type = type, Amount = amount });
            OnChanged?.Invoke();
        }

        public bool UseItem(ItemType type)
        {
            var exist = Items.Find(i => i.Type == type);
            if (exist == null || exist.Amount <= 0) return false;
            exist.Amount--;
            OnChanged?.Invoke();
            return true;
        }
    }
}
