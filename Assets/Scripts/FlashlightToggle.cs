using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightToggle : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    [Header("References")]
    public FlashlightFlicker flickerController;
    public Light flashlight;

    private PlayerInputActions inputActions;

        private bool _permanentlyDisabled = false;

    // State
    private bool flashlightOn = true;
    private bool flickerAllowed = false;
    private bool isFlickering = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.SetCallbacks(this);
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();



    public void DisableForever()
    {
        _permanentlyDisabled = true;
        flashlight.enabled = false;
        flickerController.StopFlickering();
    }

    public void OnToggleFlashlight(InputAction.CallbackContext context)
    {
        if (context.performed && !_permanentlyDisabled) return;

        flashlightOn = !flashlightOn;

        if (flashlightOn)
        {
            if (flickerAllowed)
            {
                isFlickering = true;
                StartFlickering();
            }
            else
            {
                flashlight.enabled = true;
            }
        }
        else
        {
            isFlickering = false;
            flickerAllowed = false;
            StopFlickering();
            flashlight.enabled = false;
        }
    }

    private void Update()
    {
        if (_permanentlyDisabled) return;
        {
            isFlickering = false;
            flickerAllowed = false;
            StopFlickering();
            flashlight.enabled = true;
        }
    }

    /// <summary> Wrapper so other scripts can start the flicker. </summary>
    public void StartFlickering()
    {
        flickerController.StartFlickering();
    }

    /// <summary> Wrapper so other scripts can stop the flicker. </summary>
    public void StopFlickering()
    {
        flickerController.StopFlickering();
    }

    /// <summary> Force light state and cancel any flicker. </summary>
    public void SetLight(bool on)
    {
        flashlight.enabled = on;
        if (!on)
        {
            isFlickering = false;
            flickerAllowed = false;
            StopFlickering();
            flashlightOn = false;
        }
    }
}

