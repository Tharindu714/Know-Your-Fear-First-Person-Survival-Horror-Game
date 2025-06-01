using System.Collections;
using UnityEngine;
using StarterAssets;

public class CameraFall : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the player movement script to disable during fall")]  
    public FirstPersonController playerController;

    [Tooltip("The transform you want to tilt (e.g. PlayerCameraRoot)")]
    public Transform cameraRoot;

    [Header("Fall Settings")]
    [Tooltip("How many degrees down to tilt (90 = looking at your feet)")]
    public float fallAngle = 90f;

    [Tooltip("Local Y position (relative to parent) when fully down")]
    public float fallYPosition = 0.22f;

    [Tooltip("Seconds to fall over")]
    public float fallDuration = 0.5f;

    [Header("Stand Settings")]
    [Tooltip("Seconds to stand back up")]
    public float standDuration = 0.5f;

    private Vector3   _origLocalPos;
    private Quaternion _origLocalRot;
    public bool IsDown { get; private set; } = false;

    void Start()
    {
        if (cameraRoot == null) cameraRoot = transform;
        _origLocalPos = cameraRoot.localPosition;
        _origLocalRot = cameraRoot.localRotation;
    }

    /// <summary>Call to collapse the camera to the floor over time.</summary>
    public void FallDown()
    {
        if (IsDown) return;
        StopAllCoroutines();
        StartCoroutine(FallSequence());
    }

    /// <summary>Call to stand back up from the floor.</summary>
    public void StandUp()
    {
        if (!IsDown) return;
        StopAllCoroutines();
        StartCoroutine(RiseSequence());
    }

    private IEnumerator FallSequence()
    {
        float t = 0f;
        // calculate end states
        Quaternion rotStart = cameraRoot.localRotation;
        Quaternion rotEnd   = rotStart * Quaternion.Euler(0f, 0f, fallAngle);
        Vector3 posStart    = cameraRoot.localPosition;
        Vector3 posEnd      = new Vector3(posStart.x, fallYPosition, posStart.z);

        // lerp into fall
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float pct = t / fallDuration;
            cameraRoot.localRotation = Quaternion.Slerp(rotStart, rotEnd, pct);
            cameraRoot.localPosition = Vector3.Lerp(posStart, posEnd, pct);
            yield return null;
        }
        // finalize
        cameraRoot.localRotation = rotEnd;
        cameraRoot.localPosition = posEnd;
        IsDown = true;

        // disable player controls while down
        if (playerController != null)
            playerController.enabled = false;
    }

    private IEnumerator RiseSequence()
    {
        float t = 0f;
        Quaternion rotStart = cameraRoot.localRotation;
        Quaternion rotEnd   = _origLocalRot;
        Vector3 posStart    = cameraRoot.localPosition;
        Vector3 posEnd      = _origLocalPos;

        while (t < standDuration)
        {
            t += Time.deltaTime;
            float pct = t / standDuration;
            cameraRoot.localRotation = Quaternion.Slerp(rotStart, rotEnd, pct);
            cameraRoot.localPosition = Vector3.Lerp(posStart, posEnd, pct);
            yield return null;
        }

        cameraRoot.localRotation = rotEnd;
        cameraRoot.localPosition = posEnd;
        IsDown = false;

        // re-enable player controls
        if (playerController != null)
            playerController.enabled = true;
    }
}
