using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float TrapTime;
    [SerializeField] private float TagDelay;
    [SerializeField] private float DestroyDefaultDelay;

    private Rigidbody rb;
    private bool HasHit = false;

    private void Start() {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyObject", DestroyDefaultDelay);
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.isTrigger) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HasHit = true;
            Debug.Log("Arrow Collided");
            Invoke("DestroyObject", TrapTime);
            Invoke("RemoveTag", TagDelay);
            transform.SetParent(collision.transform);

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Collider col = GetComponent<Collider>();
            if (col != null) {
                col.enabled = false;
            }
        }
    }

    private void DestroyObject() {
        Destroy(gameObject);
    }

    private void RemoveTag() {
        gameObject.tag = "Untagged";
    }

    void Update() {
        if (!HasHit) {
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }
    }
}