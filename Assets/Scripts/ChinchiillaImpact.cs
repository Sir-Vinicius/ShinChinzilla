using System;
using Unity.VisualScripting;
using UnityEngine;

public class ChinchiillaImpact : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float explosionRadius = 5f; //Raio de alcance
    [SerializeField] private float explosionForce = 500f; //Força do empurrão nos blocos
    [SerializeField] private float upwardsModifier = 1.5f; //Joga os blocos um pouco pra cima

    private void OnCollisionEnter(Collision collision)
    {
        ApplyImpactCollision(collision.contacts[0].point);
    }

    void ApplyImpactCollision(Vector3 impactPoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(impactPoint, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            Rigidbody rbBlock = hit.GetComponent<Rigidbody>();

            //Ignora o rb da chinchilla
            if (rbBlock != null && rbBlock.gameObject != gameObject)
            {
                rbBlock.AddExplosionForce(explosionForce, impactPoint, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
