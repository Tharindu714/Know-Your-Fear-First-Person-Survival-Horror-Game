using System.Collections;
using UnityEngine;
using StarterAssets; // or your FirstPersonController namespace

public class StaggerMovement : MonoBehaviour
{
    [Tooltip("Your First Person Controller component")]
    public FirstPersonController controller;

    [Tooltip("Percentage of normal speed while staggering (0–1)")]
    [Range(0.1f, 1f)]
    public float staggerSpeedFactor = 0.3f;

    [Tooltip("Seconds to stagger")]
    public float staggerDuration = 5f;

    private float _normalSpeed;

    void Start()
    {
        if (controller == null) controller = GetComponent<FirstPersonController>();
        _normalSpeed = controller.MoveSpeed;
    }

    public IEnumerator DoStagger()
    {
        controller.MoveSpeed = _normalSpeed * staggerSpeedFactor;
        yield return new WaitForSeconds(staggerDuration);
        controller.MoveSpeed = _normalSpeed;
    }
}

