using UnityEngine;

public class InventoryItemClick : MonoBehaviour
{
    private InventoryManager manager;
    private GameObject itemObject;

    public void Setup(InventoryManager manager, GameObject obj)
    {
        this.manager = manager;
        this.itemObject = obj;
    }

    // Método manual para que funcione con AR
    public void CheckClick(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                if (manager != null)
                    manager.ShowDetail(itemObject);
            }
        }
    }
}
