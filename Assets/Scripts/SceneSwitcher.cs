using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    /// <summary>
    /// Call this to pause time and load the inventory scene.
    /// </summary>
    public void OpenInventory()
    {
        // Pause all in-game activity
        Time.timeScale = 0f;

        // Load your inventory scene
        SceneManager.LoadScene("InventoryScene");
    }

    /// <summary>
    /// Call this from a button in the InventoryScene to close it and resume the game.
    /// </summary>
    public void CloseInventory()
    {
        // Unload the inventory scene
        SceneManager.UnloadSceneAsync("InventoryScene");
        SceneManager.LoadScene("Playground");

        // Resume game time
        Time.timeScale = 1f;
    }
}

