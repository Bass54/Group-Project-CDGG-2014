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
        if (Input.GetButtonDown("PlaceTrap")) {
            if (Traps > 0) {
                Instantiate(TrapPrefab, SpawnPoint.position, SpawnPoint.rotation);;
                Traps--;
                TrapText.text = Traps.ToString();
            }
        }
    }
}
