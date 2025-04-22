using UnityEngine;
using Controller;
using TMPro;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public CanvasGroup escapeBackgroundImageCanvasGroup;
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    float m_Timer;
    

    // Update is called once per frame
    void Update()
    {
        if (PinguinMover.gameLost || PinguinMover.gameWon)
        {
            endGame();
        }
    }
    void endGame()
    {
        m_Timer += Time.deltaTime;
        if (PinguinMover.gameWon)
        {
            escapeBackgroundImageCanvasGroup.alpha = m_Timer / fadeDuration;
        }
        if (PinguinMover.gameLost)
        {
            caughtBackgroundImageCanvasGroup.alpha = m_Timer / fadeDuration;
        }

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            Application.Quit();
        }
    }
}
