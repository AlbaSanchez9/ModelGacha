using UnityEngine;

public class InventoryTargetScript : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private GameObject inventoryTitle;

    public void OnTargetFound()
    {
        inventory.ShowInventory();

        if (inventoryTitle != null)
            inventoryTitle.SetActive(true);
    }

    public void OnTargetLost()
    {
        if (inventoryTitle != null)
            inventoryTitle.SetActive(false);
    }
}
