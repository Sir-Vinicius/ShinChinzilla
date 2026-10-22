using System;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class TailAttack : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private float attackForce = 80f;
    [SerializeField] private float upwardsModifier = 2f; //Joga os blocos pra cima

    [Header("Cooldown")]
    [SerializeField] private float attackCooldown = 5f;
    private float nextAttackTime = 0f;

    [Header("Verificação da GroundLayer")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint; //Ponto base nos pés da chinchila
    [SerializeField] private float groundCheckRadius = 0.4f; //Esfera de checagem do chão


    private Rigidbody rb;
    private InputSystem_Actions playerInput;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new InputSystem_Actions();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        playerInput.Player.TailAttack.performed += ctx => PerformTailAttack();
    }

    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();

    void PerformTailAttack()
    {
        if (Time.time < nextAttackTime) return; //Checa Cooldown
        if (!isGrounded()) return; //Checa se está no chão

        nextAttackTime = Time.time + attackCooldown;

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
            Vector3 randomDirection = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            impulseSource.GenerateImpulse(randomDirection * 2f);
        }

        Debug.Log("Ataque de cauda!");
    }

    bool isGrounded()
    {
        Vector3 checkPosition;

        if (groundCheckPoint != null) checkPosition = groundCheckPoint.position;
        else checkPosition = transform.position;

        return Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        //Range da Ataque de Cauda
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //Verificação do chão
        Gizmos.color = Color.green;
        Vector3 checkPosition = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}
