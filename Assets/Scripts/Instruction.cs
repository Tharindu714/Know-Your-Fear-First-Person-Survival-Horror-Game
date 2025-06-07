using UnityEngine;
using System.Collections;
using TMPro;
using StarterAssets;

public class Instruction : MonoBehaviour
{
    public static bool IsReading { get; private set; }

    [Header("UI & Mesh")]
    [SerializeField] private GameObject instructionUI;
    [SerializeField] private Renderer noteMesh;

    [Header("Player Control")]
    [SerializeField] private FirstPersonController playerController;

    [Header("Flashlight Settings")]
    [SerializeField] private FlashlightToggle flashlightToggle;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    private bool _isOpen = false;

    /// <summary>
    /// Call via your interact system to toggle the instruction note.
    /// </summary>
    public void ToggleInstruction()
    {
        _isOpen = !_isOpen;
        IsReading = _isOpen;

        instructionUI.SetActive(_isOpen);
        noteMesh.enabled = !_isOpen;
        playerController.enabled = !_isOpen;
        flashlightToggle.ForceDisable();

        // play appropriate sound
        if (_isOpen && openClip != null)
            AudioManager.I.sfxSource.PlayOneShot(openClip);
        else if (!_isOpen && closeClip != null)
            AudioManager.I.sfxSource.PlayOneShot(closeClip);
    }
}

