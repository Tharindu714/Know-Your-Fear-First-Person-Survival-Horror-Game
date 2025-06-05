using UnityEngine;
using System.Collections;

public class CameraFall : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;         // Drag your Main Camera.transform here
    public MonoBehaviour playerController; // Drag the script that controls player look (e.g., MouseLook) here

    [Header("Settings")]
    public float fallDuration = 1.5f;      // How long to animate tilt down or stand up
    public float tiltAngle = 90f;          // 90° = look straight down

    private Quaternion _originalRotation;
    private bool _isFalling = false;

    /// <summary>
    /// True while the camera is fully “fallen” (tilted down)
    /// and awaiting a StandUp() call.
    /// </summary>
    public bool IsDown { get; private set; } = false;

    private void Start()
    {
        if (playerCamera != null)
            _originalRotation = playerCamera.localRotation;
    }

    /// <summary>
    /// Begins the “fall” animation: tilts camera downward over fallDuration,
    /// disables playerController, and finally sets IsDown = true.
    /// </summary>
    public void TriggerFall()
    {
        if (!_isFalling && !_isStandingUp && playerCamera != null)
        {
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        _isFalling = true;

        // Disable player look immediately
        if (playerController != null)
            playerController.enabled = false;

        float elapsed = 0f;
        Quaternion startRot = playerCamera.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, tiltAngle);

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fallDuration);
            playerCamera.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        playerCamera.localRotation = endRot;
        IsDown = true;
        _isFalling = false;
    }

    private bool _isStandingUp = false;

    /// <summary>
    /// If the camera is down (IsDown == true), begins the “stand up” animation:
    /// tilts camera back to its original rotation over fallDuration,
    /// and re-enables playerController when done.
    /// </summary>
    public void StandUp()
    {
        if (IsDown && !_isStandingUp && playerCamera != null)
        {
            StartCoroutine(StandUpRoutine());
        }
    }

    private IEnumerator StandUpRoutine()
    {
        _isStandingUp = true;
        IsDown = false;

        float elapsed = 0f;
        Quaternion startRot = playerCamera.localRotation;
        Quaternion endRot = _originalRotation;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fallDuration);
            playerCamera.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        playerCamera.localRotation = endRot;

        // Re-enable player look
        if (playerController != null)
            playerController.enabled = true;

        _isStandingUp = false;
    }
}
