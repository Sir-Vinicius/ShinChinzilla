using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Câmeras")]
    [SerializeField] private CinemachineCamera cannonCamera; //Câmera do Launcher
    [SerializeField] private CinemachineCamera flightCamera; //Câmera Top-Down
    [SerializeField] private CinemachineCamera gameplayCamera; //Câmera de Gameplay

    [Header("Configurações da Câmera de Voo")]
    [SerializeField] private Vector3 flightCameraOffset = new Vector3(0f, 5f, -5f); //Distância da câmera no voo
    [SerializeField] private float flightCameraFollowSpeed = 3f;

    [Header("Configurações da Câmera de Gameplay")]
    [SerializeField] private Vector3 gameplayCameraOffset = new Vector3(0f, 3f, -3f); //Distância da câmera em gameplay
    [SerializeField] private float gameplayCameraFollowSpeed = 10f;

    [Header("Configuração da Transição")]
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Detecção de Pouso")]
    [SerializeField] private LayerMask groundLayer; // Defina qual layer é o chão/blocos
    private Rigidbody rb;
    private ShinSwap shinSwap;
    public bool hasLanded = false; //Verifica se já pousou
    private bool isTransitioning = false; //Controla se está no meio da transição de câmeras

    [Header("Gerenciamento da partida")]
    [SerializeField] private GameManager gameManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        shinSwap = GetComponent<ShinSwap>();
    }

    //Método chamado no CannonLauncher
    public void EnableFlightCamera()
    {
        hasLanded = false;
        isTransitioning = false;

        flightCamera.Priority = 20; //Prioridade maior para a câmera de voo
        cannonCamera.Priority = 10; //Prioridade menor para a câmera do canhão

        /* //Ativa a câmera de voo e desativa a do canhão
         if (flightCamera != null) flightCamera.SetActive(true);
         if (cannonCamera != null) cannonCamera.SetActive(false);
         if (gameplayCamera != null) gameplayCamera.SetActive(false);
         */
    }

    void LateUpdate()
    {
        /*
        //Enquanto está no ar a câmera de voo persegue a chinchila em top-down
        if (!hasLanded && flightCamera != null && flightCamera.activeSelf)
        {
            Vector3 targetPosition = transform.position + flightCameraOffset;
            flightCamera.transform.position = Vector3.Lerp(flightCamera.transform.position, targetPosition, flightCameraFollowSpeed * Time.deltaTime);

            //Câmera olha para a chinchila
            flightCamera.transform.LookAt(transform.position);
        }
        else if (isTransitioning && flightCamera != null && gameplayCamera != null)
        {
            //Calcula para onde a câmera de gameplay estaria agora
            Vector3 targetPosition = transform.position + gameplayCameraOffset;

            //Move e rotaciona a flightCamera suavemente em direção à posição da gameplay
            flightCamera.transform.position = Vector3.Lerp(flightCamera.transform.position, targetPosition, transitionSpeed * Time.deltaTime);

            //Cria uma rotação olhando para a chinchila e faz um Slerp suave até ela
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - flightCamera.transform.position);
            flightCamera.transform.rotation = Quaternion.Slerp(flightCamera.transform.rotation, targetRotation, transitionSpeed * Time.deltaTime);

         */   //Se a câmera de voo chegou perto o suficiente da posição final faz a troca
            if (hasLanded)
            {
                isTransitioning = false;
                gameplayCamera.Priority = 20; //Prioridade maior para a câmera de gameplay
                flightCamera.Priority = 10; //Prioridade menor para a câmera de voo
                                            //flightCamera.SetActive(false);
                                            //gameplayCamera.SetActive(true);
            }
     }
            


//Detecta quando a chinchila bate no chão ou nos blocos
private void OnCollisionEnter(Collision collision)
{
    if (hasLanded) return;

    //Verifica se o objeto que ela bateu pertence à camada de chão/destrutíveis
    //Ou checa se a velocidade vertical ficou próxima de zero após o lançamento
    if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground")
    {
        if (collision.relativeVelocity.magnitude > 1f) // Evita falso positivo logo ao nascer
        {
            OnLanding();
        }
    }
}

    void OnLanding()
    {
        Debug.Log("Chinchila pousou!");

        isTransitioning = true;
        hasLanded = true;

        shinSwap.Swap(); //Ativa script de movimento, etc

        if (gameManager != null)
        {
            gameManager.StartGroundTimer();
        }
        else
        {
            Debug.LogError(
                "GameManager não foi atribuído no CameraController."
            );
        }
    }

    public void ReturnToCannonCamera()
    {
        hasLanded = false;
        isTransitioning = false;

        if (flightCamera != null)
            flightCamera.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.SetActive(false);

        if (cannonCamera != null)
            cannonCamera.SetActive(true);

        Debug.Log("Câmera retornou ao canhão.");
    }
}
