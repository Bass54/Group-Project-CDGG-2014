using UnityEngine;
using TMPro;

public class BowAndArrowScript : MonoBehaviour
{
    [SerializeField] GameObject ArrowPrefab;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] private int Arrows = 10;
    [SerializeField] private TextMeshProUGUI ArrowText;

    void Start() {
        ArrowText.text = Arrows.ToString();
    }

    void Update() {
<<<<<<< HEAD
<<<<<<< HEAD
        if (Input.GetKeyDown(KeyCode.E)) {
=======
        if (Input.GetButtonDown("FireBow")) {
>>>>>>> parent of ef19dcf (Fixed the sound)
=======
        if (Input.GetButtonDown("FireBow")) {
>>>>>>> parent of ef19dcf (Fixed the sound)
            if (Arrows > 0) {
                Instantiate(ArrowPrefab, SpawnPoint.position, SpawnPoint.rotation);
                Arrows--;
                ArrowText.text = Arrows.ToString();
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Arrows")) {
            Arrows += 5;
            ArrowText.text = Arrows.ToString();
            Destroy(other.gameObject);
        }
    }
}
