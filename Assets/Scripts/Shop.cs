using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public bool randomlyGenerateAllThisShit;
    [SerializeField] private int numberOfItemsToShow = 2;
    bool generated;

    [SerializeField] private List<ShopItem> _itemsForSale;
    [SerializeField] private Vector3 _itemSpawnPointOffset = Vector3.zero;
    private ShopMenu _shopMenu;

    void Start()
    {
        _shopMenu = ShopMenu.Instance;
    }

    public bool Interact()
    {
        if (!generated && randomlyGenerateAllThisShit)
        {
            List<ShopItem> itemsToShow = GetPositionBasedSelection();
            _itemsForSale = itemsToShow;
        }

        _shopMenu.FillShopMenu(_itemsForSale);
        _shopMenu.SetItemSpawnPoint(transform.position + _itemSpawnPointOffset);
        _shopMenu.OpenMenu();

        return true;
    }

    private List<ShopItem> GetPositionBasedSelection()
    {
        if (!randomlyGenerateAllThisShit || _itemsForSale.Count <= numberOfItemsToShow)
            return new List<ShopItem>(_itemsForSale);

        Vector3 p = transform.position;
        float hash =
            Mathf.Sin(p.x * 12.9898f + p.y * 78.233f + p.z * 54.321f) * 43758.5453f;

        int seed = Mathf.FloorToInt((hash - Mathf.Floor(hash)) * 999999f);
        Random.InitState(seed);

        var sorted = _itemsForSale
            .OrderBy(item => Random.value)
            .Take(numberOfItemsToShow)
            .ToList();

        return sorted;
    }

    public List<ShopItem> GetItemsForSale()
    {
        return _itemsForSale;
    }
}
