// Assets/Scripts/Items/ItemCatalog.cs
using System.Collections.Generic;
using UnityEngine;
using Game.Inventory;

[CreateAssetMenu(menuName = "Game/Item Catalog", fileName = "ItemCatalog")]
public class ItemCatalog : ScriptableObject
{
    public List<ItemDef> items = new();

    private Dictionary<ItemType, ItemDef> _map;
    public void InitIfNeeded()
    {
        if (_map != null) return;
        _map = new Dictionary<ItemType, ItemDef>();
        foreach (var def in items)
            if (def != null) _map[def.type] = def;
    }

    public ItemDef Get(ItemType type)
    {
        InitIfNeeded();
        return _map != null && _map.TryGetValue(type, out var def) ? def : null;
    }
}
