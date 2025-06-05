using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using TMPro;


public class Toys : MonoBehaviour
{
    [Header("UI & Mesh")]
    [SerializeField] private GameObject boxUI;          // e.g. the “open box” panel
    [SerializeField] private Renderer boxMesh;          // the model to hide/show

    [Header("Player Control")]
    [SerializeField] private FirstPersonController playerController;

    [Header("Flashlight Settings")]
    [SerializeField] private FlashlightToggle flashlightToggle;
    // [SerializeField, Min(1)] private int flickerCount = 5;
    // [SerializeField, Min(0f)] private float flickerMinInterval = 0.05f;
    // [SerializeField, Min(0f)] private float flickerMaxInterval = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSfx;
    [SerializeField, Min(0f)] private float screamDelay = 0.8f;
    [SerializeField] private AudioClip screamClip;

    [Header("UI Counter")]
    [SerializeField] private TMP_Text countText;       // your “0/11” text

    [Header("Inventory")]
    [SerializeField] private ItemObject item;          // assign the NoteObject or ToyObject

    private static int _collectedCount = 0;
    private const int _totalItems = 11;

    private bool _isOpen = false;
    private bool _collected = false;

    /// <summary>
    /// Call this (e.g. via your Interaction script) to open the box UI,
    /// then close it to collect the item.
    /// </summary>

    public void OpenCloseBox()
    {
        // If already collected, do nothing
        if (_collected) return;

        // Toggle box UI on/off
        _isOpen = !_isOpen;
        boxUI.SetActive(_isOpen);
        playerController.enabled = !_isOpen;
        boxMesh.enabled = !_isOpen;

        if (_isOpen)
        {
            // On open: play sound + flicker flashlight
            if (audioSource != null && openSfx != null)
                AudioManager.I.sfxSource.PlayOneShot(openSfx);

            StopAllCoroutines();
            flashlightToggle.ForceEnable();
            Invoke(nameof(PlayScream), screamDelay);
        }
        else
        {
            // On close: collect item
            _collected = true;

            // Add to your inventory system
            InventoryManager.Instance.AddItem(item);

            // Update counter
            _collectedCount++;
            if (countText != null)
                countText.text = $"{_collectedCount}/{_totalItems}";

            // Disable collider & hide the whole box so it can't be reopened
            if (TryGetComponent<Collider>(out var col))
                col.enabled = false;
            gameObject.SetActive(false);
        }
    }

    private void PlayScream()
    {
        if (audioSource != null && screamClip != null)
            AudioManager.I.sfxSource.PlayOneShot(screamClip);
    }
}

