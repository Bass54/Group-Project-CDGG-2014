using UnityEngine;

public class ArrowPositioner : MonoBehaviour
{
    [SerializeField] private float rotationRange = 5f;
    
    void Start() {
        foreach (Transform child in transform)
        {
            RandomizeRotation(child);

            // Go one child deeper
            foreach (Transform grandchild in child)
            {
                RandomizeRotation(grandchild);
            }
        }
    }

    void RandomizeRotation(Transform target) {
        float randX = Random.Range(-rotationRange, rotationRange);
        float randY = Random.Range(-rotationRange, rotationRange);
        float randZ = Random.Range(-rotationRange, rotationRange);

        target.Rotate(new Vector3(randX, randY, randZ), Space.Self);
    }
}
