using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;

    /// <summary>Populate this slot from a NoteObject (or any ItemObject).</summary>
    public void Setup(ItemObject item)
    {
        if (item.icon != null)
            iconImage.sprite = item.icon;
        titleText.text = item.itemName;  // use a human‐readable name
    }
}

