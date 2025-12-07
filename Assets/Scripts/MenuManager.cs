using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Método que se ejecuta cuando se presiona el botón "Jugar"
    public void PlayButton()
    {
        SceneManager.LoadScene("prueba");
    }

    // Método que se ejecuta cuando se presiona el botón "Salir"
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
