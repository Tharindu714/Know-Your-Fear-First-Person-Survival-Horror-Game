using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    // Name of your gameplay scene (Playground)
   
    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("LoadingScene");
    }


    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
