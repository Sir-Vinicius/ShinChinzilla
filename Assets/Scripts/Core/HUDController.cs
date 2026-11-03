using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    [Header("Referências dos gerenciadores")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Elementos da HUD")]
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Estrelas")]
    [SerializeField] private GameObject[] stars;

    [Header("Tela de resultado")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject victoryBackground;
    [SerializeField] private GameObject defeatBackground;

    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalStarsText;

    private void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.AttemptStarted += ShowHUD;
            gameManager.AttemptEnded += HideHUD;
            gameManager.GameFinished += ShowResult;
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.AttemptStarted -= ShowHUD;
            gameManager.AttemptEnded -= HideHUD;
            gameManager.GameFinished -= ShowResult;
        }
    }
    void Start()
    {
        gameplayHUD.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameplayHUD.activeSelf)
        {
            return;
        }

        UpdateTimer();
        UpdateScore();
        UpdateStars();
    }

    private void ShowHUD()
    {
        gameplayHUD.SetActive(true);
    }

    private void HideHUD()
    {
        gameplayHUD.SetActive(false);
    }

    private void UpdateTimer()
    {
        float time = gameManager.IsAttemptTimerRunning
            ? gameManager.RemainingAttemptTime
            : gameManager.AttemptDuration;

        int seconds = Mathf.CeilToInt(time);

        timerText.text = seconds.ToString("00");
    }

    private void UpdateScore()
    {
        scoreText.text = scoreManager.TotalScore.ToString();
    }

    private void UpdateStars()
    {
        int earnedStars = scoreManager.EarnedStars;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < earnedStars);
        }
    }

    private void ShowResult(bool victory)
    {
        // Esconde a HUD de gameplay ao terminar a partida.
        HideHUD();

        // Exibe o painel de resultado.
        resultPanel.SetActive(true);

        // Ativa somente o fundo correspondente ao resultado.
        victoryBackground.SetActive(victory);
        defeatBackground.SetActive(!victory);

        // Apresenta os valores finais da partida.
        finalScoreText.text = scoreManager.TotalScore.ToString();

        int earnedStars = scoreManager.EarnedStars;

        finalStarsText.text = new string('*', earnedStars) +
                              new string('-', 3 - earnedStars);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        // Quando tiver a scene de main menu, carregar ela.
        // SceneManager.LoadScene("MainMenu");

        Application.Quit();
    }
}
