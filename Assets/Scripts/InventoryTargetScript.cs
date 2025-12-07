using UnityEngine;

public class InventoryTargetScript : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private GameObject inventoryTitle;

    // Método que se ejecuta cuando el target es detectado
    public void OnTargetFound()
    {
        inventory.ShowInventory();

        if (inventoryTitle != null)
            inventoryTitle.SetActive(true);
    }

    // Método que se ejecuta cuando el target se pierde
    public void OnTargetLost()
    {
        if (inventoryTitle != null)
            inventoryTitle.SetActive(false);
    }
}
