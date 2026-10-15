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
    [SerializeField] private Rigidbody chinchillaRb;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private Transform barrelTransform; //Tubo do canhão
    public CannonState currentState = CannonState.Aiming;

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
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float chargeSpeed;
    private float currentForce = 0f;

    [Header("UI da Barra de Força")]
    [SerializeField] private Slider powerSlider;

    //Referências para Input System
    private CannonInput cannonInput;
    private Vector2 aimInput;

    void Awake()
    {
        cannonInput = new CannonInput();

        //Leitura WASD para mirar
        cannonInput.Cannon.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        cannonInput.Cannon.Aim.canceled += ctx => aimInput = Vector2.zero;

        cannonInput.Cannon.ChargeAndLaunch.started += ctx => StartCharging();
        cannonInput.Cannon.ChargeAndLaunch.canceled += ctx => LaunchChinchilla();
    }

    private void OnEnable() => cannonInput.Enable();

    private void OnDisable() => cannonInput.Disable();

    void Start()
    {
        if (chinchillaRb != null)
        {
            chinchillaRb.gameObject.SetActive(false);
        }
        else Debug.LogError("Rigidbody do Chinchilla não encontrado.");

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
    }

    void Update()
    {
        switch (currentState)
        {
            case CannonState.Aiming:
                HandleAiming();
                break;

            case CannonState.Charging:
                HandleCharging();
                break;

            case CannonState.Launched:
                //Camera segue
                break;
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

        currentState = CannonState.Launched;

        //Esconde a barra de força após o disparo
        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
        }

        if (chinchillaRb != null)
        {
            //Ativa a Chinchila
            chinchillaRb.gameObject.SetActive(true);

            //Teleporta a Chinchila para o ponto de lançamento
            if (launchPoint != null)
            {
                chinchillaRb.transform.position = launchPoint.position;
                chinchillaRb.transform.rotation = launchPoint.rotation;
            }

            //Usa a direção para onde launchPoint está apontando
            Vector3 launchDirection = (launchPoint.forward + Vector3.up * 0.6f).normalized;
     
            chinchillaRb.AddForce(launchDirection * currentForce, ForceMode.Impulse);

            Debug.Log($"Chinchila lançada com força: {currentForce}");
        }
    }
}
