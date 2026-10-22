using System;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public static event Action<DestructibleBlock> Destroyed;

    [Header("Configurações")]
    [SerializeField] private float destroyDelay = 3f;
    [SerializeField, Min(0)] private int points = 5;
    private bool isBroken = false;

    public int Points => points;
    public bool IsBroken => isBroken;

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        if (collision.gameObject.CompareTag("Player")) BreakBlock();

    }

    public void BreakBlock()
    {
        if (isBroken) return;

        isBroken = true;

        Destroyed?.Invoke(this);

        //Adicionar VFX depois
        //Instantiate(particlesPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, destroyDelay);
    }
}
