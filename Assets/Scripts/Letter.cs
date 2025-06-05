using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using TMPro;


public class Letter : MonoBehaviour
{
    [Header("UI & Mesh")]
    [SerializeField] private GameObject letterUI;
    [SerializeField] private Renderer letterMesh;

    [Header("Player Control")]
    [SerializeField] private FirstPersonController playerController;

    [Header("Flashlight Settings")]
    [SerializeField] private FlashlightToggle flashlightToggle;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip paperFoldClip;
    [SerializeField] private AudioClip screamClip;
    [SerializeField, Min(0f)] private float screamDelay;

    [Header("UI Counter")]
    public TMP_Text countText;     // drag in your “0/12” Text object

    public ItemObject item;

    private static int _collectedCount = 0;
    private const int _totalNotes = 12;


    private bool _isOpen = false;
    private bool _collected = false;

    /// <summary>
    /// Call this (e.g. via your interaction script) to open/close the letter.
    /// </summary>
    public void OpenCloseLetter()
    {
        // If already collected, ignore further interaction
        if (_collected) return;

        // Toggle UI
        _isOpen = !_isOpen;
        letterUI.SetActive(_isOpen);
        playerController.enabled = !_isOpen;
        letterMesh.enabled = !_isOpen;

        if (_isOpen)
        {
            // On open: play sound and flicker
            AudioManager.I.sfxSource.PlayOneShot(paperFoldClip);

            StopAllCoroutines();
            StartCoroutine(LetterSequence());
        }
        else
        {
            // On close: mark as collected
            _collected = true;
            InventoryManager.Instance.AddItem(item);
            _collectedCount++;
            if (countText) countText.text = $"{_collectedCount}/{_totalNotes}";

            // Disable interaction and hide the object completely
            GetComponent<Collider>().enabled = false;
            gameObject.SetActive(false);
        }
    }

    private IEnumerator LetterSequence()
    {
        // 1) Stop any ongoing flicker logic

        // 2) Manual flicker loop
        // for (int i = 0; i < flickerCount; i++)
        // {
        //     flashlightToggle.SetLight(false);
        //     yield return new WaitForSeconds(Random.Range(flickerMinInterval, flickerMaxInterval));
        //     flashlightToggle.SetLight(true);
        //     yield return new WaitForSeconds(Random.Range(flickerMinInterval, flickerMaxInterval));
        // }
        // Ensure steady on
        flashlightToggle.ForceDisable();

        // 4) Wait then play scream
        yield return new WaitForSeconds(screamDelay);
        if (screamClip != null)
            AudioManager.I.sfxSource.PlayOneShot(screamClip);
    }
}

