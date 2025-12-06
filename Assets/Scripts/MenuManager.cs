using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("prueba");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
