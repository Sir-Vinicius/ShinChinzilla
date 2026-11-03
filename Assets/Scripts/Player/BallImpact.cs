using System;
using Unity.VisualScripting;

using UnityEngine;

public class BallImpact : MonoBehaviour
{
    [Header("Configuração de Impacto")]
    [SerializeField] private float explosionForce = 100f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Configuração de Paredes Invisíveis (Wall)")]
    [SerializeField] private float wallBounceForce = 15f;

    private bool hasImpacted = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted) return;

        if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground")
        {
            if (collision.relativeVelocity.magnitude > 1f)
            {
                hasImpacted = true;
                Debug.Log("Impacto detectado com o chão");
            }
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            ApplyImpactToBlocks(collision);
            Debug.Log("Impacto detectado com o bloco");
        }

        //Verificação para ricochete na parede invisível
        if (collision.gameObject.CompareTag("Wall"))
        {
            ApplyWallBounce(collision);
            Debug.Log("Impacto detectado com a parede");
        }
    }

    void ApplyImpactToBlocks(Collision collision)
    {
        if (hasImpacted == true) return;

        Vector3 contactPoint = collision.contacts[0].point;

        Collider[] hitColliders = Physics.OverlapSphere(contactPoint, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Block"))
            {
                Rigidbody blockRb = hit.GetComponent<Rigidbody>();

                if (blockRb != null)
                {
                    blockRb.AddExplosionForce(explosionForce, contactPoint, explosionRadius, 1f, ForceMode.Impulse);
                }

                DestructibleBlock destructible = hit.GetComponent<DestructibleBlock>();
                if (destructible != null)
                {
                    destructible.BreakBlock();
                }
            }            
        }
    }

    void ApplyWallBounce(Collision collision)
    {
        if (rb != null && collision.contacts.Length > 0)
        {
            //Pega a direção normal da parede
            Vector3 bounceDirection = collision.contacts[0].normal;

            //Leve toque para cima
            bounceDirection += Vector3.up * 0.2f;

            //Zera a velocidade atual para o impulso ser consistente
            rb.linearVelocity = Vector3.zero;

            //Aplica a força de impulso no sentido oposto à parede
            rb.AddForce(bounceDirection.normalized * wallBounceForce, ForceMode.Impulse);
        }
    }
}
