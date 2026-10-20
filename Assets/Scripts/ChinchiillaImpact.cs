using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using Random = UnityEngine.Random;

public class ChinchiillaImpact : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float explosionRadius = 5f; //Raio de alcance
    [SerializeField] private float explosionForce = 500f; //Força do empurrão nos blocos
    [SerializeField] private float upwardsModifier = 1.5f; //Joga os blocos um pouco pra cima

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            ApplyImpactCollision(collision.contacts[0].point);

            //Componente do Cinemachine que gera o efeito de shake na câmera
            if (impulseSource != null)
            {
                Vector3 randomDirection = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                impulseSource.GenerateImpulse(randomDirection);
                //Adicionar um cooldown?
            }
        }
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
