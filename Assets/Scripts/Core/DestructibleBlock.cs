using System;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public static event Action<DestructibleBlock> Destroyed;

    [Header("Configurações")]
    [SerializeField] private float destroyDelay = 3f;
    [SerializeField, Min(0)] private int points = 5;
    [SerializeField] private float minImpactVelocityToBreak = 4.0f;
    private bool isBroken = false;
    private float spawnTime;

    void Start()
    {
        //Registra o momento exato em que o bloco nasceu
        spawnTime = Time.time;
    }

    public int Points => points;
    public bool IsBroken => isBroken;

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        //Ignora colisões de impacto logo no primeiro segundo de jogo
        if (Time.time - spawnTime < 1f) return;

        //Verifica se colidiu com o Player ou com outro Bloco
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Block"))
        {
            //Verifica se a velocidade do impacto é forte o suficiente para causar danos
            if (collision.relativeVelocity.magnitude >= minImpactVelocityToBreak)
            {
                BreakBlock();
            }
        }
    }

    public void BreakBlock()
    {
        if (isBroken) return;

        isBroken = true;

        Debug.Log($"Destruição registrada em {gameObject.name}");

        Destroyed?.Invoke(this);

        if (BlockPointsSpawner.Instance != null)
        {
            BlockPointsSpawner.Instance.ShowPoints(transform.position, points);
        }

        Invoke(nameof(SpawnBreakVFX), destroyDelay);

        Destroy(gameObject, destroyDelay);
    }

    private void SpawnBreakVFX()
    {
        //Chama o VFX do bloco quebrando (índice 1 no VFXManager)
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.PlayVFX(1, transform.position, Quaternion.identity);
        }
    }
}
