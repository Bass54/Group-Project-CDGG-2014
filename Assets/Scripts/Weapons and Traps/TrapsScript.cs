using UnityEngine;
using TMPro;

public class TrapsScript : MonoBehaviour
{
    [SerializeField] GameObject TrapPrefab;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] private int Traps = 10;
    [SerializeField] private TextMeshProUGUI TrapText;
    
    void Start() {
        TrapText.text = Traps.ToString();
    }

    void Update() {
<<<<<<< HEAD
<<<<<<< HEAD
        if (Input.GetKeyDown(KeyCode.Q)) {
=======
        if (Input.GetButtonDown("PlaceTrap")) {
>>>>>>> parent of ef19dcf (Fixed the sound)
=======
        if (Input.GetButtonDown("PlaceTrap")) {
>>>>>>> parent of ef19dcf (Fixed the sound)
            if (Traps > 0) {
                Instantiate(TrapPrefab, SpawnPoint.position, SpawnPoint.rotation);
                Traps--;
                TrapText.text = Traps.ToString();
            }
        }
    }
}
