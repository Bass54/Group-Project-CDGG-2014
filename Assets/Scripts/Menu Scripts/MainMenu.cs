using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
