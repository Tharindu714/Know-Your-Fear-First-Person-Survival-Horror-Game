using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    [Header("References")]
    public Light flashlight;     // → drag the Light component of your flashlight here

    private bool isOn = true;

    private void Start()
    {
        if (flashlight != null)
            flashlight.enabled = isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlight == null) return;
        isOn = !isOn;
        flashlight.enabled = isOn;
    }

    /// <summary>Disable flashlight permanently until ForceEnable() is called.</summary>
    public void ForceDisable()
    {
        isOn = false;
        if (flashlight != null)
            flashlight.enabled = false;
    }

    /// <summary>Re-enable flashlight permanently.</summary>
    public void ForceEnable()
    {
        isOn = true;
        if (flashlight != null)
            flashlight.enabled = true;
    }
}
