using UnityEngine;
using TMPro;

public class TextHandler : MonoBehaviour
{
    public TextMeshProUGUI followerCountText;
    public TextMeshProUGUI objectiveCountText;
    
    void Start()
    {
        objectiveCountText.text = "Rescue friends";
        followerCountText.text = PlayerFollower.count.ToString() + "/5";
    }

   
    void Update()
    {
        setText();
    }
    void setText()
    {
        if (PlayerFollower.count == 5)
        {
            followerCountText.gameObject.SetActive(false);
            objectiveCountText.text = "Escape";
        }
        else
        {
            if (PlayerFollower.count < 5)
            {
                followerCountText.text = PlayerFollower.count.ToString() + "/5";

            }
        }
        
    }
}
