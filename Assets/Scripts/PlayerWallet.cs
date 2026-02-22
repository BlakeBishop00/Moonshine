using UnityEngine;
using UnityEngine.Events;

public class PlayerWallet : MonoBehaviour
{
    [HideInInspector] public UnityEvent<int> OnBalanceChanged;
    private int _currentBalance;
    public bool reset;
    public int resetBal;

    void Awake()
    {
        if (reset)
        {
            PlayerPrefs.SetInt("PlayerBalance", resetBal);
        }

        _currentBalance = PlayerPrefs.GetInt("PlayerBalance", 0);
    }

    public bool Deposit(int amount)
    {
        if (amount < 0)
            return false;

        _currentBalance += amount;
        OnBalanceChanged?.Invoke(_currentBalance);
        PlayerPrefs.SetInt("PlayerBalance", _currentBalance);
        return true;
    }

    public bool Withdraw(int amount)
    {
        if (amount < 0 || amount > _currentBalance)
            return false;

        _currentBalance -= amount;
        OnBalanceChanged?.Invoke(_currentBalance);
        PlayerPrefs.SetInt("PlayerBalance", _currentBalance);
        return true;
    }

    public int GetBalance()
    {
        return _currentBalance;
    }
}
