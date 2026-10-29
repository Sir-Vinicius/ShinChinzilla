using System;
using Unity.VisualScripting;

using UnityEngine;

public class BallImpact : MonoBehaviour
{
    [Header("Configuração de Impacto")]
    [SerializeField] private float explosionForce = 100f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask groundLayer;

    private bool hasImpacted = false;

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
}
