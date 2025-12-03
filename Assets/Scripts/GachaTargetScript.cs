using UnityEngine;

public class GachaTargetScript : MonoBehaviour
{
    [SerializeField] private GameObject botonTirar;
    [SerializeField] private GameObject botonAnuncio;
    [SerializeField] private GameObject feecbackMonedas;

    private void Awake()
    {
        if (botonTirar != null)
            botonTirar.SetActive(false); // Empieza oculto

        if (botonAnuncio != null)
            botonAnuncio.SetActive(false);

        if (feecbackMonedas != null)
            feecbackMonedas.SetActive(false);
    }

    // Se llama automáticamente desde Vuforia cuando el target se detecta
    public void OnTargetFound()
    {
        if (botonTirar != null)
            botonTirar.SetActive(true);

        if (botonAnuncio != null)
            botonAnuncio.SetActive(true);

        if (feecbackMonedas != null)
            feecbackMonedas.SetActive(true);
    }

    // Se llama automáticamente desde Vuforia cuando el target se pierde
    public void OnTargetLost()
    {
        if (botonTirar != null)
            botonTirar.SetActive(false);

        if (botonAnuncio != null)
            botonAnuncio.SetActive(false);

        if (feecbackMonedas != null)
            feecbackMonedas.SetActive(false);
    }
}
