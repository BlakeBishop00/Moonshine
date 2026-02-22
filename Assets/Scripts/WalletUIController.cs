using TMPro;
using UnityEngine;

public class WalletUIController : MonoBehaviour
{
    private TextMeshProUGUI _walletText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _walletText = GetComponent<TextMeshProUGUI>();
        PlayerController.Instance.PlayerWallet.OnBalanceChanged.AddListener(UpdateWalletDisplay);
        UpdateWalletDisplay(PlayerController.Instance.PlayerWallet.GetBalance());
    }

    private void UpdateWalletDisplay(int newBalance)
    {
        _walletText.text = $"${newBalance}";
    }
}
