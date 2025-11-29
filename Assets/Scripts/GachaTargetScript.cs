using UnityEngine;

public class GachaTargetScript : MonoBehaviour
{
    [SerializeField] private GameObject botonTirar;

    private void Awake()
    {
        if (botonTirar != null)
            botonTirar.SetActive(false); // Empieza oculto
    }

    // Se llama automáticamente desde Vuforia cuando el target se detecta
    public void OnTargetFound()
    {
        if (botonTirar != null)
            botonTirar.SetActive(true);
    }

    // Se llama automáticamente desde Vuforia cuando el target se pierde
    public void OnTargetLost()
    {
        if (botonTirar != null)
            botonTirar.SetActive(false);
    }
}
