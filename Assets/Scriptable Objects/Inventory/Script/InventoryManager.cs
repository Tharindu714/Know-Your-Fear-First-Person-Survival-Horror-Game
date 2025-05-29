using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // All collected items
    private readonly List<ItemObject> _items = new List<ItemObject>();

    // Fires whenever an item’s added
    public UnityEvent<ItemObject> onItemCollected = new UnityEvent<ItemObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>Adds the item if not already in inventory.</summary>
    public void AddItem(ItemObject item)
    {
        if (item == null || _items.Contains(item)) return;
        _items.Add(item);
        onItemCollected.Invoke(item);
    }

    public IReadOnlyList<ItemObject> Items => _items;
}

