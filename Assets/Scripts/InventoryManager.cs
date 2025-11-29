using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    private List<GameObject> collectedPrefabs = new List<GameObject>();
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject backButton;

    private GameObject currentDetailObject = null;


    public static InventoryManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (backButton != null)
            backButton.SetActive(false);
    }

    private void Update()
    {
        // Detectar click / toque sobre objetos del inventario
        if (currentDetailObject == null) // Solo si estamos en modo inventario
        {
            Ray ray = Camera.main.ScreenPointToRay(
                Mouse.current != null ? (Vector3)Mouse.current.position.ReadValue() :
                Touchscreen.current != null ? (Vector3)Touchscreen.current.primaryTouch.position.ReadValue() :
                Vector3.zero
            );

            if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame))
            {
                foreach (InventoryItemClick clickHandler in inventoryContainer.GetComponentsInChildren<InventoryItemClick>())
                {
                    clickHandler.CheckClick(ray);
                }
            }
        }
    }

    // Añadir premio al inventario
    public void AddItem(GameObject itemPrefab)
    {
        if (!collectedPrefabs.Contains(itemPrefab))
        {
            collectedPrefabs.Add(itemPrefab);
        }
    }

    // Mostrar todos los premios en el InventoryTarget
    public void ShowInventory()
    {
        // Limpiar contenido previo
        foreach (Transform child in inventoryContainer)
            Destroy(child.gameObject);

        currentDetailObject = null;

        // Parámetros configurables
        int columns = 3;                 // Número de columnas de tu grid
        float spacing = 0.35f;           // Distancia entre premios
        Vector3 startPos = Vector3.zero; // Centro del grid

        for (int i = 0; i < collectedPrefabs.Count; i++)
        {
            GameObject prefab = collectedPrefabs[i];

            int row = i / columns;    // Fila
            int col = i % columns;    // Columna

            // Posición dentro del grid
            Vector3 pos = new Vector3(
                (col - (columns - 1) / 2f) * spacing,
                0.05f,
                (row * spacing)
            );

            GameObject obj = Instantiate(prefab, inventoryContainer);
            obj.transform.localPosition = startPos + pos;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one * 0.3f; // Ajusta tamaño si hace falta

            // Desactivar interacción mientras estamos en inventario
            PrizeController controller = obj.GetComponent<PrizeController>();
            if (controller != null)
                controller.enabled = false;

            // Añadir collider para detectar clicks
            Collider colObj = obj.GetComponent<Collider>();
            if (colObj == null)
                obj.AddComponent<BoxCollider>();

            // Añadir botón de detalle
            obj.AddComponent<InventoryItemClick>().Setup(this, obj);
        }

        if (backButton != null)
            backButton.SetActive(false);
    }

    // Modo detalle de un objeto
    public void ShowDetail(GameObject item)
    {
        // Ocultar todos los demás
        foreach (Transform child in inventoryContainer)
        {
            if (child.gameObject != item)
                child.gameObject.SetActive(false);
        }

        // Activar interacción del premio
        PrizeController controller = item.GetComponent<PrizeController>();
        if (controller != null)
            controller.enabled = true;

        item.transform.localPosition = Vector3.zero; // Centrar
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one * 0.5f; // Escala más grande

        currentDetailObject = item;

        if (backButton != null)
            backButton.SetActive(true);
    }


    // Volver al inventario
    public void BackToInventory()
    {
        if (currentDetailObject != null)
        {
            // Desactivar interacción del detalle
            PrizeController controller = currentDetailObject.GetComponent<PrizeController>();
            if (controller != null)
                controller.enabled = false;

            currentDetailObject = null;
        }

        // Volver a mostrar y reordenar todos los objetos
        int columns = 3;
        float spacing = 0.35f;
        Vector3 startPos = Vector3.zero;

        int i = 0;
        foreach (Transform child in inventoryContainer)
        {
            child.gameObject.SetActive(true);

            int row = i / columns;
            int col = i % columns;

            Vector3 pos = new Vector3(
                (col - (columns - 1) / 2f) * spacing,
                0.05f,
                (row * spacing)
            );

            child.localPosition = startPos + pos;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one * 0.3f;

            // Asegurarse de que PrizeController esté desactivado
            PrizeController pc = child.GetComponent<PrizeController>();
            if (pc != null)
                pc.enabled = false;

            i++;
        }

        if (backButton != null)
            backButton.SetActive(false);
    }

    public bool TieneItem(GameObject itemPrefab)
    {
        return collectedPrefabs.Contains(itemPrefab);
    }
}
