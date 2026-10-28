using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        WaitingForLaunch,
        InAttempt,
        ReturningToLauncher,
        Victory,
        Defeat
    }

    [Header("Configurações da partida")]
    [SerializeField, Min(1)]
    private int maxAttempts = 3;

    private int attemptsUsed;
    private GameState currentState = GameState.WaitingForLaunch;

    public int AttemptsUsed => attemptsUsed;
    public int AttemptsRemaining => maxAttempts - attemptsUsed;
    public GameState CurrentState => currentState;

    public bool StartAttempt()
    {
        if (currentState != GameState.WaitingForLaunch &&
            currentState != GameState.ReturningToLauncher)
        {
            return false;
        }

        if (attemptsUsed >= maxAttempts)
        {
            Debug.Log("Não há mais tentativas disponíveis.");
            return false;
        }

        attemptsUsed++;
        currentState = GameState.InAttempt;

        Debug.Log(
            $"Tentativa {attemptsUsed}/{maxAttempts} iniciada."
        );

        return true;
    }

    public void EndAttempt()
    {
        if (currentState != GameState.InAttempt)
        {
            return;
        }

        currentState = GameState.ReturningToLauncher;

        Debug.Log(
            $"Tentativa {attemptsUsed}/{maxAttempts} encerrada."
        );
    }

    public void FinishGame(bool victory)
    {
        if (currentState == GameState.Victory ||
            currentState == GameState.Defeat)
        {
            return;
        }

        currentState = victory
            ? GameState.Victory
            : GameState.Defeat;

        Debug.Log(
            victory ? "Fase concluída: VITÓRIA!" :
                      "Fase concluída: DERROTA!"
        );
    }
}
