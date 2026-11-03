using UnityEngine;
using UnityEngine.InputSystem;

public class ShinSwap : MonoBehaviour
{
    [Header("Configurações da Troca")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject chinchillaObject;
    [SerializeField] private GameObject ballObject;
    [SerializeField] private float chinchillaMass = 50f;
    private bool hasSwapped = false;

    private Rigidbody chinchillaRb;
    private PlayerController playerController;
    private TailAttack tailAttack;

    private CameraController cameraController;


    void Start()
    {
        chinchillaRb = GetComponent<Rigidbody>();
        tailAttack = GetComponent<TailAttack>();
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>();

        if (playerController != null) playerController.enabled = false; //desabilita o PlayerController
        if (tailAttack != null) tailAttack.enabled = false; //desabilita o TailAttack
    }

    public void Swap()
    {
       if (chinchillaRb == null) return;

        // verifica se está quase sem rotação angular
        if (cameraController.hasLanded)
        {
            hasSwapped = true;

            ballObject.SetActive(false);
            chinchillaObject.SetActive(true);

            if (chinchillaRb != null) chinchillaRb.mass = chinchillaMass;
            if (chinchillaObject != null) chinchillaObject.transform.localEulerAngles = Vector3.zero;
            if (playerController != null) playerController.enabled = true; //Habilita o PlayerController
            if (tailAttack != null) tailAttack.enabled = true; //Habilita o TailAttack

            Debug.Log("Transformação feita");
        }
    }

    public void ReturnToBall()
    {
        if (chinchillaRb == null) return;

        hasSwapped = false;

        if (playerController != null)
            playerController.enabled = false;

        if (tailAttack != null)
            tailAttack.enabled = false;

        if (chinchillaObject != null)
            chinchillaObject.SetActive(false);

        if (ballObject != null)
            ballObject.SetActive(true);

        chinchillaRb.linearVelocity = Vector3.zero;
        chinchillaRb.angularVelocity = Vector3.zero;
        chinchillaRb.useGravity = false;

        Debug.Log("Chinchila retornou à forma de bola.");
    }
}
