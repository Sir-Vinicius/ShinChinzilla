using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string PedroScene;
    public void Play()
    {
        //Seleção jogar
        SceneManager.LoadScene(PedroScene);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
