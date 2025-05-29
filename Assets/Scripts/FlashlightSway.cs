using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 1.5f;       // degrees of roll/pitch
    public float swaySmoothness = 8f;     // how fast it follows input

    private Quaternion originalRot;

    void Start()
    {
        originalRot = transform.localRotation;
    }

    void Update()
    {
        // read mouse delta
        Vector2 delta = Mouse.current.delta.ReadValue() * Time.deltaTime;

        // calculate target rotation offsets
        float xRot = Mathf.Clamp(delta.y * swayAmount, -swayAmount, swayAmount);
        float yRot = Mathf.Clamp(-delta.x * swayAmount, -swayAmount, swayAmount);

        Quaternion targetRot = originalRot *
            Quaternion.Euler(xRot, yRot, 0f);

        // lerp to smooth
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, targetRot, Time.deltaTime * swaySmoothness);
    }
}
