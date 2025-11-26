using System.Collections;
using UnityEngine;

public class Gacha : MonoBehaviour
{

    private GachaItem[] items;          // la lista de items se crea por código
    private GameObject[] ballPrefabs;     // "plantilla" de bola que creamos en Start()

    [Header("Bolas")]
    [SerializeField] private Transform spawnPoint;       // punto donde aparecerán las bolas

    [Header("Palanca")]
    [SerializeField] private Animator palancaAnimator;

    [Header("Puerta")]
    [SerializeField] private Animator doorAnimator;

    [SerializeField] private float bolaDelay = 0.3f; // delay entre palanca y bola

    void Start()
    {
        LoadBallPrefabs();   // Cargar las bolas de diferentes rarezas
        CreateItems();       // Crear la lista de premios
    }

    public void TirarPalanca()
    {
        StartCoroutine(TirarConDelay(bolaDelay));
    }

    private IEnumerator TirarConDelay(float delay)
    {
        // 1️⃣ Activar animación de la palanca
        if (palancaAnimator != null)
            palancaAnimator.SetTrigger("Pull");

        // Abrir puerta
        if (doorAnimator != null)
            doorAnimator.SetBool("isOpen", true);

        // 2️⃣ Esperar un poco
        yield return new WaitForSeconds(delay);

        // 3️⃣ Instanciar bola
        Roll();
    }

    private void LoadBallPrefabs()
    {
        ballPrefabs = new GameObject[3];
        ballPrefabs[0] = Resources.Load<GameObject>("Prefabs/Ball_3");
        ballPrefabs[1] = Resources.Load<GameObject>("Prefabs/Ball_4");
        ballPrefabs[2] = Resources.Load<GameObject>("Prefabs/Ball_5");

        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (ballPrefabs[i] == null)
                Debug.LogError($"No se encontró el prefab de bola de rareza {i + 3} estrellas...");
            else
                Debug.Log($"Bola de {i + 3} estrellas cargada correctamente.");
        }
    }

    // Creamos los objetos del gacha por código
    private void CreateItems()
    {
        items = new GachaItem[3];

        items[0] = new GachaItem("Espada", 3, Resources.Load<GameObject>("Prefabs/Espada"));
        items[1] = new GachaItem("Robot", 4, Resources.Load<GameObject>("Prefabs/Robot"));
        items[2] = new GachaItem("Dragón", 5, Resources.Load<GameObject>("Prefabs/Dragon"));
    }

    // Método público que inicia una tirada
    public void Roll()
    {
        int rarity = GetRandomRarity();
        Debug.Log("Ha salido una bola de " + rarity + " estrellas");

        // Elegir el prefab correcto según la rareza
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
            bc.Initialize(rarity, selectedItem, spawnPoint, doorAnimator); // spawnPoint = Transform del marcador AR
        else
            Debug.LogError("❌ El prefab de la bola no tiene BallController.");
    }

    private int GetRandomRarity()
    {
        float roll = Random.value;
        if (roll < 0.7f) return 3;
        if (roll < 0.95f) return 4;
        return 5;
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
            Debug.LogWarning("⚠️ No hay items con rareza " + rarity + ". Devolviendo el primero disponible.");
            return items.Length > 0 ? items[0] : null;
        }

        return filtered[Random.Range(0, filtered.Length)];
    }
}
