using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // if you use the new Input System

public class CandleLightController : MonoBehaviour
{
    [Header("Light Settings")]
    public Light candleLight;
    public Color offColor = Color.black;
    public Color litColor = new Color32(0xF3, 0xAD, 0x56, 0xFF);

    [HideInInspector] public bool isLit = false;

    void Start()
    {
        candleLight.color = offColor;
    }

    public void ToggleCandle()
    {
        isLit = !isLit;
        candleLight.color = isLit ? litColor : offColor;
    }

    /// <summary>Turn off instantly without toggling back on.</summary>
    public void Extinguish()
    {
        if (!isLit) return;
        isLit = false;
        candleLight.color = offColor;
    }
}


