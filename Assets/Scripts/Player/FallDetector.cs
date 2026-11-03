using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("Configurações de Queda e Checkpoint")]
    [SerializeField] private Transform checkpoint;
    [SerializeField] private float fallLimitY = -20f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < fallLimitY) Respawn();
    }

    void Respawn()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else Debug.LogWarning("Rigidbody não encontrado no FallDetector.");  

        if (checkpoint != null) transform.position = checkpoint.position;
        else Debug.LogWarning("Checkpoint não encontrado no FallDetector.");
    }
}
