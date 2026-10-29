using System;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CannonLauncher : MonoBehaviour
{
    public enum CannonState {  Aiming, Charging, Launched}

    [Header("Referêncas")]
    [SerializeField] private GameObject ballObject;
    [SerializeField] private GameObject chinchillaObject;
    [SerializeField] private Rigidbody chinchillaRb;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private Transform barrelTransform; //Tubo do canhão
    [SerializeField] private LineRenderer lineRenderer; //Linha da trajetória
    public CannonState currentState = CannonState.Aiming;

    [SerializeField] private GameManager gameManager;

    [Header("Configurações de Mira")]
    [SerializeField] private float horinzontalSpeed = 35f;
    [SerializeField] private float verticalSpeed = 35f;
    [SerializeField] private float minVerticalAngle = 15f;
    [SerializeField] private float maxVerticalAngle = 70f;
    [SerializeField] private float minHorizontalAngle = -45f;
    [SerializeField] private float maxHorizontalAngle = 45f;

    private float currentHorizontalAngle = 0f; //Rotação Horizontal A/D
    private float currentVerticalAngle = 0f; //Rotação Vertical W/S

    [Header("Configurações de Força")]
    [SerializeField] private float chinchillaInitialMass = 1f;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float chargeSpeed;
    private float currentForce = 0f;

    [Header("Configuração da Trajetória")]
    [SerializeField] private int trajectoryPointCount = 30; //Quantidade de pontos da trajetória
    [SerializeField] private float timeStep = 0.1f; //Intervalo de tempo entre cada ponto da trajetória

    [Header("UI da Barra de Força")]
    [SerializeField] private Slider powerSlider;

    //Referências para Input System
    private CannonInput cannonInput;
    private Vector2 aimInput;
    private bool shouldLaunch = false; //Bool para sincronizar o tiro com a física

    void Awake()
    {
        cannonInput = new CannonInput();

        //Leitura WASD para mirar
        cannonInput.Cannon.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        cannonInput.Cannon.Aim.canceled += ctx => aimInput = Vector2.zero;
    }

    private void OnEnable()
    {
        cannonInput.Enable();

        if (gameManager != null)
            gameManager.AttemptEnded += OnAttemptEnded;
    }

    private void OnDisable()
    {
        cannonInput.Disable();

        if (gameManager != null)
            gameManager.AttemptEnded -= OnAttemptEnded;
    }

    private void OnAttemptEnded()
    {
        if (chinchillaRb == null || launchPoint == null)
        {
            Debug.LogError(
                "Rigidbody da chinchila ou Launch Point não configurado."
            );
            return;
        }

        // Interrompe o movimento e retorna à posição inicial.
        chinchillaRb.linearVelocity = Vector3.zero;
        chinchillaRb.angularVelocity = Vector3.zero;
        chinchillaRb.useGravity = false;

        // Retorna à forma de bola.
        ShinSwap shinSwap = chinchillaRb.GetComponent<ShinSwap>();

        if (shinSwap != null)
            shinSwap.ReturnToBall();

        if (ballObject != null)
            ballObject.SetActive(false);

        // Retorna à câmera do canhão.
        CameraController cameraController =
            chinchillaRb.GetComponent<CameraController>();

        if (cameraController != null)
            cameraController.ReturnToCannonCamera();

        // Prepara o próximo lançamento.
        currentForce = minForce;
        currentState = CannonState.Aiming;
        shouldLaunch = false;

        if (powerSlider != null)
            powerSlider.gameObject.SetActive(false);

        if (lineRenderer != null)
            lineRenderer.positionCount = 0;

        Debug.Log("Canhão pronto para a próxima tentativa.");
    }

    void Start()
    {
        if (ballObject != null) ballObject.SetActive(false);
        if (chinchillaObject != null) chinchillaObject.SetActive(false);

        if (chinchillaRb != null) chinchillaRb.mass = chinchillaInitialMass;
        else Debug.LogError("Objeto da bola não encontrado.");

        currentHorizontalAngle = transform.eulerAngles.y;

        if (barrelTransform != null) currentVerticalAngle = barrelTransform.localEulerAngles.x;
        else Debug.LogError("Transform do tubo do canhão não encontrado.");

        //Esconde (e configura)a barra de força no início da partida
        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
            powerSlider.minValue = 0f;
            powerSlider.maxValue = 1f;
        }

        if (lineRenderer != null) lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (currentState == CannonState.Aiming && cannonInput.Cannon.ChargeAndLaunch.WasPressedThisFrame())
        {
            StartCharging();
        }

        if (currentState == CannonState.Charging && cannonInput.Cannon.ChargeAndLaunch.WasReleasedThisFrame())
        {
            shouldLaunch = true;
        }

        switch (currentState)
        {
            case CannonState.Aiming:
                HandleAiming();
                DrawTrajectory(minForce); //Mostra uma prévia com a força mínima
                break;

            case CannonState.Charging:
                HandleCharging();
                DrawTrajectory(currentForce); //Atualiza a linha conforme carrega a força!
                break;

            case CannonState.Launched:
                break;
        }
    }

    void FixedUpdate()
    {
        if (shouldLaunch)
        {
            shouldLaunch = false;
            LaunchChinchilla(); 
        }
    }

    void HandleAiming()
    {
        //A/D alteram currentHorizontalAngle (gira o corpo do tanque para os lados)
        currentHorizontalAngle += aimInput.x * horinzontalSpeed * Time.deltaTime;
        currentHorizontalAngle = Mathf.Clamp(currentHorizontalAngle, minHorizontalAngle, maxHorizontalAngle);
        transform.rotation = Quaternion.Euler(0f, currentHorizontalAngle, 0f);

        //W/S alteram currentVerticalAngle (gira o tubo do canhão para cima e para baixo)
        if (barrelTransform != null)
        {
        currentVerticalAngle -= aimInput.y * verticalSpeed * Time.deltaTime;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        barrelTransform.localRotation = Quaternion.Euler(currentVerticalAngle, 0f, 0f);
        }
    }

    void StartCharging()
    {
        if (currentState == CannonState.Aiming)
        {
            currentState = CannonState.Charging;
            currentForce = minForce; //Inicia a força de lançamento

            //Mostra e zera a barra de força quando começa a segurar o espaço
            if (powerSlider != null)
            {
                powerSlider.gameObject.SetActive(true);
                powerSlider.value = 0f;
            }
        }
    }

    void HandleCharging()
    {
        currentForce += chargeSpeed * Time.deltaTime;
        currentForce = Mathf.Clamp(currentForce, minForce, maxForce);

        //Calcula a porcentagem atual da força
        if (powerSlider != null)
        {
            float normalizedPower = (currentForce - minForce) / (maxForce - minForce);
            powerSlider.value = normalizedPower;
        }
    }

    void LaunchChinchilla()
    {
        if (currentState != CannonState.Charging) return;

        if (gameManager == null)
        {
            Debug.LogError("GameManager não foi atribuído no CannonLauncher.");
            return;
        }

        if (!gameManager.StartAttempt()) return;

        currentState = CannonState.Launched;

        //Limpa e some com a linha de trajetória ao atirar
        if (lineRenderer != null) lineRenderer.positionCount = 0;

        //Esconde a barra de força após o disparo
        if (powerSlider != null) powerSlider.gameObject.SetActive(false);

        if (ballObject != null)
        {          
            //Ativa a bola
            ballObject.gameObject.SetActive(true);

            //Teleporta a Chinchila para o ponto de lançamento
            if (launchPoint != null)
            {
                chinchillaRb.useGravity = true;
                chinchillaRb.transform.position = launchPoint.position;
                chinchillaRb.transform.rotation = launchPoint.rotation;
            }

            //Reseta  para garantir precisão absoluta
            chinchillaRb.linearVelocity = Vector3.zero;
            chinchillaRb.angularVelocity = Vector3.zero;

            //Usa a direção para onde launchPoint está apontando
            Vector3 launchDirection = (launchPoint.forward + Vector3.up * 0.6f).normalized;
     
            chinchillaRb.AddForce(launchDirection * currentForce, ForceMode.Impulse);

            CameraController camController = chinchillaRb.GetComponent<CameraController>();
            if (camController != null) camController.EnableFlightCamera();

            Debug.Log($"Chinchila lançada com força: {currentForce}");
        }
    }

    void DrawTrajectory(float forceToUse)
    {
        if (lineRenderer == null || launchPoint == null) return;

        lineRenderer.positionCount = trajectoryPointCount;
        Vector3[] points = new Vector3[trajectoryPointCount];

        //Posição inicial do lançamento
        Vector3 startPosition = launchPoint.position;

        //Velocidade inicial simulada(considerando a massa 1f do início)
        Vector3 launchDirection = (launchPoint.forward + Vector3.up * 0.6f).normalized;
        Vector3 startVelocity = launchDirection * (forceToUse / chinchillaInitialMass);

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * timeStep;

            Vector3 point = startPosition + startVelocity * t + 0.5f * Physics.gravity * t * t;
            points[i] = point;

            if (i > 0 && Physics.Linecast(points[i - 1], points[i], out RaycastHit hit))
            {
                lineRenderer.positionCount = i + 1;
                points[i] = hit.point;
                break;
            }
        }

        lineRenderer.SetPositions(points);
    }
}
