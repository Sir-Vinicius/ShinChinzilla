using System;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float destroyDelay = 2f;
    private bool isBroken = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        if (collision.gameObject.CompareTag("Player")) BreakBlock();

    }

    void BreakBlock()
    {
        isBroken = true;

        //Adicionar VFX depois
        //Instantiate(particlesPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, destroyDelay);
    }
}
