using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndHandling : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject DeathEndImage;
    [SerializeField] private Image DeathEndOpImage;
    [SerializeField] private Image DeathEndOpBack;
    [SerializeField] private GameObject WinEndImage;
    [SerializeField] private Image WinEndOpImage;
    [SerializeField] private Image WinEndOpBack;
    [SerializeField] private float FadeSpeed = 0.1f;
    private bool EndScaling = false;
    private bool Lose = false;
    private bool Win = false;
    private float opacity = 0f;

    [Header("Objects")]
    [SerializeField] private GameObject Player;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private Transform[] FollowerPenguins;
    [SerializeField] private Transform[] Enemies;
    [SerializeField] private float ClosingDistance = 1f;

    void Update() {
        if (EndScaling) {
            opacity += FadeSpeed * Time.deltaTime;
            if (Lose) {
                DeathEndImage.SetActive(true);
                SetDeathOpacity(opacity);
            }
            if (Win) {
                WinEndImage.SetActive(true);
                SetWinOpacity(opacity);
            }
        }

        CheckDistance();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject == Player) {
            Win = true;
            EndScaling = true;
            Debug.Log("Collision");
        }
    }

    public void DeathEnd() {
        Lose = true;
        EndScaling = true;
    }

    private void SetDeathOpacity(float alpha) {
        Color color = DeathEndOpImage.color;
        color.a = Mathf.Clamp01(alpha);
        DeathEndOpImage.color = color;

        Color colorBack = DeathEndOpBack.color;
        colorBack.a = Mathf.Clamp01(alpha);
        DeathEndOpBack.color = colorBack;
    }

    private void SetWinOpacity(float alpha) {
        Color color = WinEndOpImage.color;
        color.a = Mathf.Clamp01(alpha);
        WinEndOpImage.color = color;

        Color colorBack = WinEndOpBack.color;
        colorBack.a = Mathf.Clamp01(alpha);
        WinEndOpBack.color = colorBack;
    }

    void CheckDistance() {
        foreach (Transform obj in Enemies) {
            float distance = Vector3.Distance(obj.position, PlayerTransform.position);
            if (distance <= ClosingDistance) {
                DeathEnd();
            }
        }
    }
}
