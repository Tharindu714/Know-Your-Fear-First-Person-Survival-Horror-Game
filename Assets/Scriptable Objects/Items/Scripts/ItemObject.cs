using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Notes,
    Toys,
    Cursed_Toys,
    Default
}
public abstract class ItemObject : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    public Sprite icon;         // <-- new field
    public GameObject prefab;
    public ItemType type;
    [TextArea(15, 20)] public string description;
}
