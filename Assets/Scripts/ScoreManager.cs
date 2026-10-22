using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int totalScore;
    private int destroyedObjects;

    public int TotalScore => totalScore;
    public int DestroyedObjects => destroyedObjects;

    private void OnEnable()
    {
        DestructibleBlock.Destroyed += RegisterDestruction;
    }

    private void OnDisable()
    {
        DestructibleBlock.Destroyed -= RegisterDestruction;
    }

    private void RegisterDestruction(DestructibleBlock block)
    {
        totalScore += block.Points;
        destroyedObjects++;

        Debug.Log(
            $"Objeto destruído! " +
            $"Pontos: +{block.Points} | " +
            $"Pontuação total: {totalScore} | " +
            $"Destruições: {destroyedObjects}"
        );
    }
}
