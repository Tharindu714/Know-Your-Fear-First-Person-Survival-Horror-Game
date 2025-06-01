using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResManager : MonoBehaviour
{
    // Start is called before the first frame update
void Start()
{
    Screen.SetResolution(1920, 1080, false); // false = windowed, true = fullscreen
}
    /// <summary>
    /// Call this to set the game resolution.
    /// </summary>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <param name="fullscreen">True for fullscreen, false for windowed</param>
    public void SetResolution(int width, int height, bool fullscreen)
    {
        Screen.SetResolution(width, height, fullscreen);
    }

}
