using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 2.5f; //Aumenta a gravidade artificialmente
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; //Desliga a gravidade padrão da Unity
    }

    void FixedUpdate()
    {
        //Aplica uma força de gravidade personalizada mais forte baseada na massa
        Vector3 customGravity = Physics.gravity * gravityMultiplier;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }
}