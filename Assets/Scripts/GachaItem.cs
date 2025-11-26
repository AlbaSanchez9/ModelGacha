using UnityEngine;

public class GachaItem
{
    private string name;
    private int rarity;
    private GameObject item;

    public GachaItem (string name, int rarity, GameObject item)
    {
        this.name = name;
        this.rarity = rarity;
        this.item = item;
    }

    public string GetName()
    {
        return name;
    }

    public int GetRarity()
    {
        return rarity;
    }

    public GameObject GetPrefab()
    {
        return item;
    }
}
