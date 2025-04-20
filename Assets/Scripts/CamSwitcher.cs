using System.Collections;
using UnityEngine;
using Controller;

public class CamSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera cutsceneCamera;
    public float cutsceneDuration = 5f;

    public GameObject objectToMove;
    public Transform targetPosition;
    public float moveDuration = 3f;

    private bool cutscenePlayed;

    public GameObject objectiveText;
    

    void Start()
    {
        cutscenePlayed = false;
        mainCamera.enabled = true;
        cutsceneCamera.enabled = false;
    }
    void Update()
    {
        if (PlayerFollower.count == 5 && !cutscenePlayed)
        {
            TriggerCutscene();
            cutscenePlayed = true;
        }
        if (PinguinMover.gameWon || PinguinMover.gameLost)
        {
            objectiveText.SetActive(false);
        }
        
    }
    void TriggerCutscene()
    {
        StartCoroutine(SwitchToCutscene());
    }

    private IEnumerator SwitchToCutscene()
    {
        objectiveText.SetActive(false);
        mainCamera.enabled = false;
        cutsceneCamera.enabled = true;
        

  
        if (objectToMove != null && targetPosition != null)
        {
            StartCoroutine(MoveObjectOverTime(objectToMove.transform, targetPosition.position, moveDuration));
        }

    
        yield return new WaitForSeconds(cutsceneDuration);

        objectiveText.SetActive(true);
        cutsceneCamera.enabled = false;
        mainCamera.enabled = true;
    }

    private IEnumerator MoveObjectOverTime(Transform obj, Vector3 destination, float duration)
    {
        Vector3 startPos = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.position = Vector3.Lerp(startPos, destination, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = destination; 
    }
}

