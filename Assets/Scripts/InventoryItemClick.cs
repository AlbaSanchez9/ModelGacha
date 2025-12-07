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

    // Método para detectar el clic usando un Ray para que funcione con AR
    public void CheckClick(Ray ray)
    {
        // Lanza un rayo y verifica si colisiona con algún objeto
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
