using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    private GachaItem[] items;          // la lista de items se crea por código
    private GameObject[] ballPrefabs;     //plantilla de bola 

    [SerializeField] private Transform spawnPoint;       // punto donde aparecerán las bolas
    [SerializeField] private Transform prizePoint;

    [SerializeField] private Animator palancaAnimator;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator ballAnimator;

    [SerializeField] private GameObject maquinaCompleta;
    [SerializeField] private Animator maquinaAnimator;

    [SerializeField] private GameObject botonVolver;
    [SerializeField] private GameObject botonTirar;
    [SerializeField] private GameObject botonExit;

    [SerializeField] private float bolaDelay = 0.3f; // delay entre palanca y bola

    private GameObject premioActual;
    private bool bolaEnJuego = false;

    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;

    [SerializeField] private GameObject botonVerAnuncio; // Botón para ver anuncio
    [SerializeField] private int monedas = 0;
    [SerializeField] private int costoPorTirada = 1;
    [SerializeField] private TMPro.TextMeshProUGUI monedasText;

    [SerializeField] private GameObject feedbackPanelMonedas;
    [SerializeField] private TMPro.TextMeshProUGUI feedbackTextMonedas;

    [SerializeField] private GameObject particles3Star;
    [SerializeField] private GameObject particles4Star;
    [SerializeField] private GameObject particles5Star;
    private List<GameObject> activeParticles = new List<GameObject>();

    void Start()
    {
        //MusicManager.Instance.LoadMusic("MiMusicaGacha");
        //MusicManager.Instance.Play();

        LoadBallPrefabs();   // Cargar las bolas de diferentes rarezas
        CreateItems();       // Crear la lista de premios
        botonVolver.SetActive(false);

        if (botonVerAnuncio != null)
        {
            botonVerAnuncio.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(VerAnuncio);
        }

        ActualizarUI();
    }

    // Tirar del gacha
    public void TirarPalanca()
    {
        if (monedas < costoPorTirada)
        {
            ShowFeedbackMonedasDispo("No tienes suficientes monedas. Mira un anuncio para conseguir más.");
        }
        else if (!bolaEnJuego)
        {
            monedas -= costoPorTirada;
            ActualizarUI();
            AudioManager.Instance.PlayClickGacha();
            StartCoroutine(TirarConDelay(bolaDelay));
        }
    }

    private IEnumerator TirarConDelay(float delay)
    {
        bolaEnJuego = true;

        // Activar animación de la palanca
        if (palancaAnimator != null)
            palancaAnimator.SetTrigger("Pull");

        // Abrir puerta
        if (doorAnimator != null)
            doorAnimator.SetBool("isOpen", true);

        // Esperar 
        yield return new WaitForSeconds(delay);

        //Instanciar bola
        Roll();
    }

    // Cargar las bolas
    private void LoadBallPrefabs()
    {
        ballPrefabs = new GameObject[3];
        ballPrefabs[0] = Resources.Load<GameObject>("Prefabs/Bola3");
        ballPrefabs[1] = Resources.Load<GameObject>("Prefabs/Bola4");
        ballPrefabs[2] = Resources.Load<GameObject>("Prefabs/Bola5");

        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (ballPrefabs[i] == null)
                Debug.LogError($"No se encontró el prefab de bola de rareza {i + 3} estrellas...");
            else
                Debug.Log($"Bola de {i + 3} estrellas cargada correctamente.");
        }
    }

    // Crear los premios 
    private void CreateItems()
    {
        items = new GachaItem[3];

        items[0] = new GachaItem("Botella y Copa", 3, Resources.Load<GameObject>("Prefabs/BotellaYCopa"));
        items[1] = new GachaItem("Tren", 4, Resources.Load<GameObject>("Prefabs/TrenAR"));
        items[2] = new GachaItem("Bom", 5, Resources.Load<GameObject>("Prefabs/FiguraBom"));
    }

    // Método que inicia una tirada
    public void Roll()
    {
        int rarity = GetRandomRarity();
        Debug.Log("Ha salido una bola de " + rarity + " estrellas");

        // Elegir el prefab según la rareza
        GameObject ballPrefab = GetBallPrefabByRarity(rarity);
        if (ballPrefab == null)
        {
            Debug.LogError("No se encontró prefab de bola para rareza " + rarity);
            return;
        }

        // Instanciar la bola en el spawn point
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);

        // Seleccionar un item de esa rareza
        GachaItem selectedItem = GetRandomItemByRarity(rarity);

        // Inicializar el BallController de la bola
        BallController bc = ball.GetComponent<BallController>();
        if (bc != null)
            bc.Initialize(rarity, selectedItem, spawnPoint, doorAnimator, this, prizePoint); // spawnPoint = Transform del marcador AR
        else
            Debug.LogError("El prefab de la bola no tiene BallController.");
    }

    // Probabilidad de tipo de premio
    private int GetRandomRarity()
    {
        float roll = Random.value; // Random.value devuelve un float entre 0.0 y 1.0
        Debug.Log(roll);
        if (roll < 0.7f) return 3; // 70% de probabilidad de rareza 3
        if (roll < 0.95f) return 4;// 25% de probabilidad de rareza 4 (0.7 → 0.95)
        return 5; // 5% de probabilidad de rareza 5 (0.95 → 1.0)
    }

    private GameObject GetBallPrefabByRarity(int rarity)
    {
        switch (rarity)
        {
            case 3: return ballPrefabs[0];
            case 4: return ballPrefabs[1];
            case 5: return ballPrefabs[2];
            default: return null;
        }
    }

    private GachaItem GetRandomItemByRarity(int rarity)
    {
        var filtered = System.Array.FindAll(items, i => i.GetRarity() == rarity);

        if (filtered == null || filtered.Length == 0)
        {
            Debug.LogWarning("No hay items con rareza " + rarity + ". Devolviendo el primero disponible.");
            return items.Length > 0 ? items[0] : null;
        }

        return filtered[Random.Range(0, filtered.Length)];
    }

    //Mostrar premio
    public void MostrarPremio(GameObject premio)
    {
        // Guardamos el premio para destruirlo al volver
        premioActual = premio;
        HideMachine();
        if (botonTirar != null)
            botonTirar.SetActive(false);
        if (botonVerAnuncio != null)
            botonVerAnuncio.SetActive(false);
        if (monedasText != null)
            monedasText.gameObject.SetActive(false);
        if (botonExit != null)
            botonExit.SetActive(false);
    }

    public void HideMachine()
    {
        botonVolver.SetActive(true);
        if (maquinaAnimator != null)
            maquinaAnimator.SetTrigger("Desaparece");
        else
            maquinaCompleta.SetActive(false);
    }

    public void ShowMachine()
    {
        botonVolver.SetActive(false);
        if (maquinaAnimator != null)
            maquinaAnimator.SetTrigger("Aparece");
        else
            maquinaCompleta.SetActive(true);

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", false);
            doorAnimator.Play("IdlePuerta", 0, 0f);
            doorAnimator.Update(0f);
        }

        bolaEnJuego = false;
    }

    // Reset
    public void ResetToMachine()
    {
        if (premioActual != null)
        {
            Destroy(premioActual);
            premioActual = null;
        }

        if (feedbackPanel != null && feedbackPanel.activeSelf)
            feedbackPanel.SetActive(false);

        ShowMachine();

        if (botonTirar != null)
            botonTirar.SetActive(true);

        if (botonVerAnuncio != null)
            botonVerAnuncio.SetActive(true);

        if (botonExit != null)
            botonExit.SetActive(true);

        if (monedasText != null)
            monedasText.gameObject.SetActive(true);

        foreach (var p in activeParticles)
        {
            if (p != null)
                Destroy(p);
        }
        activeParticles.Clear();
    }

    // Feedback 
    public void ShowFeedback(string mensaje)
    {
        StopAllCoroutines();        // Por si estaba mostrando otro mensaje antes
        StartCoroutine(ShowFeedbackRoutine(feedbackPanel, feedbackText, mensaje));
    }

    // Feedback monedas disponibles
    public void ShowFeedbackMonedasDispo(string mensaje)
    {
        StopAllCoroutines();        // Por si estaba mostrando otro mensaje antes
        StartCoroutine(ShowFeedbackRoutine(feedbackPanelMonedas, feedbackTextMonedas, mensaje));
    }

    private IEnumerator ShowFeedbackRoutine(GameObject panel, TMPro.TextMeshProUGUI textComponent, string mensaje)
    {
        if (panel == null || textComponent == null) yield break;

        textComponent.text = mensaje;
        panel.SetActive(true);

        yield return new WaitForSeconds(3f);

        panel.SetActive(false);
    }


    //Mostrar anuncio
    private void VerAnuncio()
    {
        AdsManager.instance.ShowRewardedAd();
    }

    public void DarMonedas(int cantidad)
    {
        monedas += cantidad;
        ShowFeedbackMonedasDispo("¡Has conseguido " + cantidad + " monedas!");
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (monedasText != null)
            monedasText.text = "Monedas: " + monedas;
    }

    //Mostrar partículas
    public void SpawnParticles(int rarity, Vector3 position)
    {
        GameObject prefab = null;
        switch (rarity)
        {
            case 3: prefab = particles3Star; break;
            case 4: prefab = particles4Star; break;
            case 5: prefab = particles5Star; break;
        }

        if (prefab != null)
        {
            GameObject particles = Instantiate(prefab, position, Quaternion.identity);
            activeParticles.Add(particles);
        }
    }
}
