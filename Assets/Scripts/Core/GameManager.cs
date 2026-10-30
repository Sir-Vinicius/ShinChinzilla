using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Estados
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

    [Header("Referências")]
    [SerializeField] private ScoreManager scoreManager;

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
        // Atualiza o timer da tentativa apenas quando a partida está em andamento.
        if (!attemptTimerRunning || currentState != GameState.InAttempt)
        {
            return;
        }

        // Atualiza o tempo restante usando o tempo decorrido entre os frames.
        remainingAttemptTime -= Time.deltaTime;

        // Arredonda para cima para exibir os segundos restantes como números inteiros.
        int seconds = Mathf.CeilToInt(
            Mathf.Max(remainingAttemptTime, 0f)
        );

        // Exibe o tempo somente quando o segundo mostrado muda.
        if (seconds != lastLoggedSecond)
        {
            lastLoggedSecond = seconds;
            Debug.Log($"Tempo restante: {seconds}s");
        }

        // Encerra a tentativa quando o tempo chega a zero.
        if (remainingAttemptTime <= 0f)
        {
            remainingAttemptTime = 0f;
            attemptTimerRunning = false;

            EndAttempt();
        }
    }

    public bool StartAttempt()
    {
        // Só permite iniciar uma tentativa nos estados apropriados.
        if (currentState != GameState.WaitingForLaunch &&
            currentState != GameState.ReturningToLauncher)
        {
            return false;
        }

        // Impede que o jogador ultrapasse o limite de tentativas.
        if (attemptsUsed >= maxAttempts)
        {
            Debug.Log("Não há mais tentativas disponíveis.");
            return false;
        }

        // Registra a nova tentativa e atualiza o estado da partida.
        attemptsUsed++;
        currentState = GameState.InAttempt;

        Debug.Log(
            $"Tentativa {attemptsUsed}/{maxAttempts} iniciada."
        );

        return true;
    }

    public void StartGroundTimer()
    {
        // O temporizador só começa durante uma tentativa e não pode ser iniciado duas vezes.
        if (currentState != GameState.InAttempt ||
            attemptTimerRunning)
        {
            return;
        }

        // Inicializa a contagem quando a chinchila pousa.
        remainingAttemptTime = attemptDuration;
        lastLoggedSecond = -1;
        attemptTimerRunning = true;

        Debug.Log(
            $"Timer iniciado: {attemptDuration:F0} segundos."
        );
    }

    public void EndAttempt()
    {
        // Evita encerrar novamente uma tentativa que já não está ativa.
        if (currentState != GameState.InAttempt)
        {
            return;
        }

        attemptTimerRunning = false;

        // Verifica se esta foi a última tentativa disponível.
        if (attemptsUsed >= maxAttempts)
        {
            if (scoreManager == null)
            {
                Debug.LogError("ScoreManager não foi atribuído.");
                return;
            }

            // A partida é vencida quando o progresso alcança pelo menos 40%.
            bool victory = scoreManager.DestructionProgress >= 40f;

            FinishGame(victory);
            return;
        }

        // Ainda existem tentativas: prepara o retorno ao canhão.
        currentState = GameState.ReturningToLauncher;

        Debug.Log(
            $"Tentativa {attemptsUsed}/{maxAttempts} encerrada."
        );

        // Notifica os componentes inscritos para que possam executar suas rotinas de retorno.
        AttemptEnded?.Invoke();
    }

    public void FinishGame(bool victory)
    {
        // Impede que uma partida já concluída seja finalizada novamente.
        if (currentState == GameState.Victory ||
            currentState == GameState.Defeat)
        {
            return;
        }

        // Interrompe o temporizador e registra o resultado final.
        attemptTimerRunning = false;

        currentState = victory
            ? GameState.Victory
            : GameState.Defeat;

        // Pausa o tempo do jogo após definir o resultado.
        Time.timeScale = 0f;

        Debug.Log(
            victory ? "Fase concluída: VITÓRIA!" :
                      "Fase concluída: DERROTA!"
        );
    }
}
