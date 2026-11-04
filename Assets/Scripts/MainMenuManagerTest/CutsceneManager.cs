using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Images")]
    [SerializeField] private Image cutsceneImage;
    [SerializeField] private Sprite[] cutsceneImages;

    [Header("Navigation Buttons")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button playButton;

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 0.4f;

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    private int currentImageIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        if (cutsceneImages == null || cutsceneImages.Length == 0)
        {
            Debug.LogError("Nenhuma imagem foi configurada para a cutscene!");
            return;
        }

        ShowCurrentImage();

        playButton.gameObject.SetActive(false);
    }

    private void ShowCurrentImage()
    {
        cutsceneImage.sprite = cutsceneImages[currentImageIndex];

        previousButton.interactable = currentImageIndex > 0;

        nextButton.interactable =
            currentImageIndex < cutsceneImages.Length - 1;

        playButton.gameObject.SetActive(
            currentImageIndex == cutsceneImages.Length - 1
        );
    }

    public void NextImage()
    {
        if (isTransitioning ||
            currentImageIndex >= cutsceneImages.Length - 1)
            return;

        StartCoroutine(ChangeImage(currentImageIndex + 1));
    }

    public void PreviousImage()
    {
        if (isTransitioning || currentImageIndex <= 0)
            return;

        StartCoroutine(ChangeImage(currentImageIndex - 1));
    }

    private IEnumerator ChangeImage(int targetIndex)
    {
        isTransitioning = true;

        float elapsed = 0f;
        Color imageColor = cutsceneImage.color;

        // Fade Out
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = 1f - (elapsed / fadeDuration);

            cutsceneImage.color = new Color(
                imageColor.r,
                imageColor.g,
                imageColor.b,
                alpha
            );

            yield return null;
        }

        currentImageIndex = targetIndex;

        ShowCurrentImage();

        // Fade In
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = elapsed / fadeDuration;

            cutsceneImage.color = new Color(
                imageColor.r,
                imageColor.g,
                imageColor.b,
                alpha
            );

            yield return null;
        }

        cutsceneImage.color = imageColor;

        isTransitioning = false;
    }

    public void SkipCutscene()
    {
        LoadGameplay();
    }

    public void PlayGame()
    {
        LoadGameplay();
    }

    private void LoadGameplay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
}
