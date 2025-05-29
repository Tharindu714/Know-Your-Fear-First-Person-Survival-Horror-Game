using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;            // Entire inventory panel
    [SerializeField] private Transform contentParent;     // ScrollView → Content
    [SerializeField] private GameObject slotPrefab;       // NoteSlotPrefab
    [SerializeField] private TMP_Text countText;          // “3 / 12 Notes”

    [Header("Config")]
    [SerializeField] private int totalNotes = 12;

    void Start()
    {
        // Populate existing items (if any)
        foreach (var item in InventoryManager.Instance.Items)
            AddSlot(item);

        // Listen for new collections
        InventoryManager.Instance.onItemCollected.AddListener(AddSlot);

        UpdateCounter();
    }

    void AddSlot(ItemObject item)
    {
        var go = Instantiate(slotPrefab, contentParent);
        go.GetComponent<InventorySlot>().Setup(item);
        UpdateCounter();
    }

    void UpdateCounter()
    {
        int have = InventoryManager.Instance.Items.Count;
        countText.text = $"{have} / {totalNotes} Notes";
    }

    /// <summary>Call this from a UI button or key press to toggle visibility.</summary>
    public void ToggleVisibility()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
