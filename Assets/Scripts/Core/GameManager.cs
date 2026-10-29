using UnityEngine;
using System;

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

    [Header("Configurações do timer")]
    [SerializeField, Min(1f)]
    private float attemptDuration = 15f;

    private int attemptsUsed;
    private GameState currentState = GameState.WaitingForLaunch;

    private float remainingAttemptTime;
    private bool attemptTimerRunning;
    private int lastLoggedSecond = -1;

    public event Action AttemptEnded;

    public int AttemptsUsed => attemptsUsed;
    public int AttemptsRemaining => maxAttempts - attemptsUsed;
    public GameState CurrentState => currentState;

    public float RemainingAttemptTime => remainingAttemptTime;
    public bool IsAttemptTimerRunning => attemptTimerRunning;

    private void Update()
    {
        if (!attemptTimerRunning || currentState != GameState.InAttempt)
        {
            return;
        }

        remainingAttemptTime -= Time.deltaTime;

        int seconds = Mathf.CeilToInt(
            Mathf.Max(remainingAttemptTime, 0f)
        );

        if (seconds != lastLoggedSecond)
        {
            lastLoggedSecond = seconds;
            Debug.Log($"Tempo restante: {seconds}s");
        }

        if (remainingAttemptTime <= 0f)
        {
            remainingAttemptTime = 0f;
            attemptTimerRunning = false;

            EndAttempt();
        }
    }

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

    public void StartGroundTimer()
    {
        if (currentState != GameState.InAttempt ||
            attemptTimerRunning)
        {
            return;
        }

        remainingAttemptTime = attemptDuration;
        lastLoggedSecond = -1;
        attemptTimerRunning = true;

        Debug.Log(
            $"Timer iniciado: {attemptDuration:F0} segundos."
        );
    }

    public void EndAttempt()
    {
        if (currentState != GameState.InAttempt)
        {
            return;
        }

        attemptTimerRunning = false;
        currentState = GameState.ReturningToLauncher;

        Debug.Log(
            $"Tentativa {attemptsUsed}/{maxAttempts} encerrada."
        );

        AttemptEnded?.Invoke();
    }

    public void FinishGame(bool victory)
    {
        if (currentState == GameState.Victory ||
            currentState == GameState.Defeat)
        {
            return;
        }

        attemptTimerRunning = false;

        currentState = victory
            ? GameState.Victory
            : GameState.Defeat;

        Debug.Log(
            victory ? "Fase concluída: VITÓRIA!" :
                      "Fase concluída: DERROTA!"
        );
    }
}
