using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop Item")]
public class ShopItem : ScriptableObject
{
    public GameObject ItemPrefab;
    public Texture Icon;
    public string ItemName;
    public string Description;
    public int Cost;
}

