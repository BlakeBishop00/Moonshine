using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public static ShopMenu Instance { get; private set; }
    [SerializeField] private ShopItemUI _shopItemUIPrefab;
    [SerializeField] private GameObject _shopMenuPanel;
    [SerializeField] private Transform _shopItemUIParent;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _closeButton;
    private ShopItem _currentShopItem;
    private Vector3 _itemSpawnPoint;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _buyButton.onClick.AddListener(BuyCurrentItem);
        _closeButton.onClick.AddListener(CloseMenu);
    }

    public void FillShopMenu(List<ShopItem> itemsForSale)
    {
        _shopItemUIParent.GetComponentsInChildren<ShopItemUI>(true).ToList().ForEach(ui => Destroy(ui.gameObject));

        foreach (ShopItem item in itemsForSale)
        {
            Debug.Log(item.ItemName);
            GameObject shopItemUI = Instantiate(_shopItemUIPrefab.gameObject, _shopItemUIParent);
            shopItemUI.GetComponent<ShopItemUI>().Initialize(this, item);
        }

        SetShopItem(null);
    }

    public void SetItemSpawnPoint(Vector3 point)
    {
        _itemSpawnPoint = point;
    }

    public void OpenMenu()
    {
        _shopMenuPanel.SetActive(true);
        _closeButton.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.Instance.PlayerMovement.enabled = false;
    }

    public void CloseMenu()
    {
        _shopMenuPanel.SetActive(false);
        _closeButton.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.Instance.PlayerMovement.enabled = true;
    }

    public void SetShopItem(ShopItem shopItem)
    {
        _currentShopItem = shopItem;
        if (shopItem == null)
        {
            _itemNameText.text = "";
            _itemDescriptionText.text = "";
            _itemPriceText.text = "";
            return;
        }

        _itemNameText.text = shopItem.ItemName;
        _itemDescriptionText.text = shopItem.Description;
        _itemPriceText.text = $"${shopItem.Cost}";
    }

    public void BuyCurrentItem()
    {
        if (_currentShopItem == null)
            return;

        if (PlayerController.Instance.PlayerWallet.Withdraw(_currentShopItem.Cost))
        {
            Instantiate(_currentShopItem.ItemPrefab, _itemSpawnPoint, Quaternion.identity);
            FMODUnity.RuntimeManager.PlayOneShot("event:/NotAffectedByReverb/UI/Buy");
        }
        else
        {
            Debug.Log("Not enough money to buy this item.");
        }
    }
}
