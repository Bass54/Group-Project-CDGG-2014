using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public float loadTime;
    private float timer;
    private bool starting;

    void Update()
    {
        if(starting == true)
        {
            Debug.Log(timer);
            timer -= Time.deltaTime;
            PlayGame();
        }
    }

    public void PlayGame()
    {
        if (timer <= 0 && starting == false)
        {
            timer = loadTime;
            starting = true;
        }
        else if (timer <= 0 && starting == true)
        {
            starting = false;
            SceneManager.LoadScene(1);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
