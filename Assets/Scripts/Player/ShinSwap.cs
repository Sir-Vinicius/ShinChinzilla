using UnityEngine;
using UnityEngine.InputSystem;

public class ShinSwap : MonoBehaviour
{
    [Header("Configurações da Troca")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject chinchillaObject;
    [SerializeField] private GameObject ballObject;
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

            if (chinchillaRb != null) chinchillaRb.mass = 50f;
            if (chinchillaObject != null) chinchillaObject.transform.localEulerAngles = Vector3.zero;
            if (playerController != null) playerController.enabled = true; //Habilita o PlayerController
            if (tailAttack != null) tailAttack.enabled = true; //Habilita o TailAttack

            Debug.Log("Transformação feita");
        }
    }
}
