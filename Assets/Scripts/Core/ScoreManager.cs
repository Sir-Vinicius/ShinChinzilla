using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Configurações de progresso")]
    [SerializeField, Range(0f, 1f)]
    private float destructionTarget = 0.8f;

    private int totalScore;
    private int destroyedObjects;
    private int initialObjectCount;
    private int destructionGoal;

    public int TotalScore => totalScore;
    public int DestroyedObjects => destroyedObjects;
    public int InitialObjectCount => initialObjectCount;
    public int DestructionGoal => destructionGoal;

    // Calcula o progresso em porcentagem, limitado ao intervalo de 0% a 100%.
    // Se não houver uma meta válida, retorna 0%.
    public float DestructionProgress =>
        destructionGoal > 0 ? Mathf.Clamp01((float)destroyedObjects / destructionGoal) * 100f: 0f;

    // Quantidade de estrelas obtidas com base no progresso da destruição.
    public int EarnedStars =>
        DestructionProgress >= 80f ? 3 :
        DestructionProgress >= 60f ? 2 :
        DestructionProgress >= 40f ? 1 : 0;

    private void Awake()
    {
        // Registra a quantidade inicial de objetos antes de começar a partida.
        CountInitialObjects();
    }

    private void OnEnable()
    {
        Debug.Log("ScoreManager ativado e inscrito no evento.");
        DestructibleBlock.Destroyed += RegisterDestruction;
    }

    private void OnDisable()
    {
        DestructibleBlock.Destroyed -= RegisterDestruction;
    }

    private void CountInitialObjects()
    {
        // Localiza todos os objetos destrutíveis existentes na cena.
        DestructibleBlock[] blocks =
            FindObjectsByType<DestructibleBlock>(
                FindObjectsSortMode.None
            );

        initialObjectCount = blocks.Length;

        // Calcula quantos objetos precisam ser destruídos para atingir a meta configurada.
        // CeilToInt arredonda para cima quando o resultado não é um número inteiro.
        destructionGoal = Mathf.CeilToInt(
            initialObjectCount * destructionTarget
        );

        Debug.Log(
            $"Objetos iniciais: {initialObjectCount} | " +
            $"Meta de destruição: {destructionGoal}"
        );
    }

    private void RegisterDestruction(DestructibleBlock block)
    {
        // Adiciona os pontos definidos no objeto destruído e atualiza a contagem.
        totalScore += block.Points;
        destroyedObjects++;

        Debug.Log(
            $"Objeto destruído! " +
            $"Pontos: +{block.Points} | " +
            $"Pontuação total: {totalScore} | " +
            $"Destruições: {destroyedObjects}" +
            $"Progresso: {DestructionProgress:F1}%" +
            $"Estrelas: {EarnedStars}"
        );
    }
}
