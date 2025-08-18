using UnityEngine;
using Game.Inventory;

[CreateAssetMenu(menuName = "Game/Item Def", fileName = "ItemDef_")]
public class ItemDef : ScriptableObject
{
    public ItemType type;          // MaxHp, FoxFire, Honbul...
    public string displayName;     // UI 표시명
    public Sprite icon;            // 인벤토리 아이콘
    [TextArea] public string desc; // 설명(선택)
    public bool stackable = true;  // 스택 가능 여부(선택)
}