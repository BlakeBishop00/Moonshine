using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    private ShopMenu _shopMenu;
    private ShopItem _shopItem;

    public void Initialize(ShopMenu shopMenu, ShopItem shopItem)
    {
        _shopMenu = shopMenu;
        _shopItem = shopItem;

        GetComponent<RawImage>().texture = shopItem.Icon;
        GetComponent<Button>().onClick.AddListener(OnItemClicked);
    }

    private void OnItemClicked()
    {
        _shopMenu.SetShopItem(_shopItem);
    }
}
