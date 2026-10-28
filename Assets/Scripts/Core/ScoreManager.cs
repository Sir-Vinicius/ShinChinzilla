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

    public float DestructionProgress =>
        destructionGoal > 0 ? Mathf.Clamp01((float)destroyedObjects / destructionGoal) * 100f: 0f;

    public int EarnedStars =>
        DestructionProgress >= 80f ? 3 :
        DestructionProgress >= 60f ? 2 :
        DestructionProgress >= 40f ? 1 : 0;

    private void Awake()
    {
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
        DestructibleBlock[] blocks =
            FindObjectsByType<DestructibleBlock>(
                FindObjectsSortMode.None
            );

        initialObjectCount = blocks.Length;

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
        Debug.Log($"ScoreManager recebeu destruição: {block.name}");
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
