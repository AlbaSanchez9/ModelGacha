using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    private int rarity;
    private GachaItem containedItem;
    private bool isOpened = false;

    [SerializeField] private Animator animator; // Animator para animaciones
    [SerializeField] private Transform parentMarker; // El marcador AR donde se coloca la bola

    private Animator doorAnimator;
    private Gacha gachaManager;
    private Transform prizePoint;

    // -------------------------------
    // Inicializar bola desde Gacha
    // -------------------------------
    public void Initialize(int rarity, GachaItem item, Transform marker, Animator door, Gacha manager,Transform prizePoint)
    {
        this.rarity = rarity;
        this.containedItem = item;
        this.parentMarker = marker;
        this.doorAnimator = door;
        this.gachaManager = manager;
        this.prizePoint = prizePoint;

        // Posicionar la bola sobre el marcador AR
        transform.SetParent(parentMarker);
        transform.localPosition = Vector3.up * 0.1f; // 10 cm sobre la tarjeta
        transform.localRotation = Quaternion.identity;

        UpdateAppearance();
    }

    // -------------------------------
    // Cambiar apariencia según rareza
    // -------------------------------
    private void UpdateAppearance()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            switch (rarity)
            {
                case 3: rend.material.color = Color.cyan; break;
                case 4: rend.material.color = Color.purple; break;
                case 5: rend.material.color = Color.yellow; break;
            }
        }
    }

    // -------------------------------
    // Detectar toque en pantalla
    // -------------------------------
    private void Update()
    {
        if (isOpened) return;

        // Para touch en móvil
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                    OpenBall();
            }
        }

        // Para click con ratón en ordenador
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                    OpenBall();
            }
        }
    }

    // Evita que el toque sobre UI active la bola
    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.position;
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    // -------------------------------
    // Abrir bola y mostrar premio
    // -------------------------------
    private void OpenBall()
    {
        isOpened = true;

        if (animator != null)
            animator.SetTrigger("Open");

        GameObject premio = null;
        if (containedItem != null && containedItem.GetPrefab() != null)
        {
            bool esNuevo = !InventoryManager.Instance.TieneItem(containedItem.GetPrefab());

            premio = Instantiate(containedItem.GetPrefab(), prizePoint.position, Quaternion.identity);
            premio.transform.SetParent(prizePoint);

            InventoryManager.Instance.AddItem(containedItem.GetPrefab());

            if (esNuevo)
                AudioManager.Instance.PlayNuevoPremio();
            else
                AudioManager.Instance.PlayPremioRepetido();
        }

        if (doorAnimator != null)
            doorAnimator.SetBool("isOpen", false);

        if (gachaManager != null)
        {
            gachaManager.MostrarPremio(premio);
        }

        Destroy(gameObject, 0.5f);
    }
}

