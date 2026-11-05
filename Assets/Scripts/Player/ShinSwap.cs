using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class ShinSwap : MonoBehaviour
{
    [Header("Configurações da Troca")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject chinchillaObject;
    [SerializeField] private GameObject ballObject;
    [SerializeField] private float chinchillaMass = 50f;
    [SerializeField] private float landingDelay = 1f;
    private bool hasSwapped = false;

    [SerializeField] private MMF_Player swapSound;
    [Header("Explosão de Impacto")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackForce = 150f;
    [SerializeField] private float upwardsModifier = 15f; //Joga os blocos pra cima

    private Rigidbody chinchillaRb;
    private PlayerController playerController;
    private TailAttack tailAttack;
    private CameraController cameraController;
    private CinemachineImpulseSource impulseSource;


    void Start()
    {
        chinchillaRb = GetComponent<Rigidbody>();
        tailAttack = GetComponent<TailAttack>();
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (playerController != null) playerController.enabled = false; //desabilita o PlayerController
        if (tailAttack != null) tailAttack.enabled = false; //desabilita o TailAttack

    }

    public void Swap()
    {
       if (chinchillaRb == null) return;
       if (hasSwapped) return;

        // verifica se está quase sem rotação angular
        if (cameraController.hasLanded)
        {
            hasSwapped = true;

            //Ativa VFX
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayVFX(0, transform.position, Quaternion.identity);
                Debug.LogWarning("VFX");
            }

            //Faz a troca do modelo bola para a chinchila
            ballObject.SetActive(false);
            chinchillaObject.SetActive(true);

            if (chinchillaRb != null) chinchillaRb.mass = chinchillaMass;
            if (chinchillaObject != null) chinchillaObject.transform.localEulerAngles = Vector3.zero;

            ImpactExplosion(); 

            StartCoroutine(EnableControlsRoutine());
            

            Debug.Log("Transformação feita");

            PlaySwapSound();
        }
    }

    private IEnumerator EnableControlsRoutine()
    {
        yield return new WaitForSeconds(landingDelay);

        //Habilita os scripts de controlo após o tempo de pouso
        if (playerController != null) playerController.enabled = true; //Habilita o PlayerController
        if (tailAttack != null) tailAttack.enabled = true; //Habilita o TailAttack
    }

    void ImpactExplosion()
    {
        Vector3 attackCenter = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(attackCenter, attackRange);

        foreach (Collider hit in hitColliders)
        {
            Rigidbody rbBlock = hit.GetComponent<Rigidbody>();

            if (rbBlock != null && hit.CompareTag("Block"))
            {
                Vector3 direction = (hit.transform.position - attackCenter).normalized;
                rbBlock.AddExplosionForce(attackForce, attackCenter, attackRange, upwardsModifier, ForceMode.Impulse);
            }

            DestructibleBlock destructible = hit.GetComponent<DestructibleBlock>();
            if (destructible != null)
            {
                destructible.BreakBlock();
            }
        }

        if (impulseSource != null)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-0.6f, 0.6f), Random.Range(-0.6f, 0.6f));
            impulseSource.GenerateImpulse(randomDirection * 2f);
        }
    }

    public void ReturnToBall()
    {
        if (chinchillaRb == null) return;

        hasSwapped = false;

        StopAllCoroutines();

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

    public void PlaySwapSound()
    {


        if (swapSound != null)
        {

            swapSound.PlayFeedbacks();
        }

    }
}
