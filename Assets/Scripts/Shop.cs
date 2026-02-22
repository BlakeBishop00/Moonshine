using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ShopItem> _itemsForSale;
    [SerializeField] private Vector3 _itemSpawnPointOffset = Vector3.zero;
    private ShopMenu _shopMenu;

    void Start()
    {
        _shopMenu = ShopMenu.Instance;
    }

    public bool Interact()
    {
        _shopMenu.FillShopMenu(_itemsForSale);
        _shopMenu.SetItemSpawnPoint(transform.position + _itemSpawnPointOffset);
        _shopMenu.OpenMenu();

        return true;
    }

    public List<ShopItem> GetItemsForSale()
    {
        return _itemsForSale;
    }
}
