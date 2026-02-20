using UnityEngine;

public class AICharacterBase : MonoBehaviour, IInspectable, IInteractable
{
    [SerializeField] private float _baseSellValue = 1000f;
    [SerializeField] private float _ageMultiplier = 0.2f;
    private IngredientData _preferredMixture;

    void Start()
    {
        _preferredMixture = ScriptableObject.CreateInstance<IngredientData>();
        _preferredMixture.SweetValue = Random.Range(0f, 100f);
        _preferredMixture.SourValue = Random.Range(0f, 100f); 
        _preferredMixture.BitterValue = Random.Range(0f, 100f);
        _preferredMixture.SaltyValue = Random.Range(0f, 100f);
        _preferredMixture.UmamiValue = Random.Range(0f, 100f);
    }

    public int CalculateSellValue(IngredientData mixture, int ageLevel)
    {
        // 5 Dimensional distance from preferred mixture plus a bonus for age level
        float x1 = Mathf.Pow(_preferredMixture.SweetValue - mixture.SweetValue, 2);
        float x2 = Mathf.Pow(_preferredMixture.SourValue - mixture.SourValue, 2);
        float x3 = Mathf.Pow(_preferredMixture.BitterValue - mixture.BitterValue, 2);
        float x4 = Mathf.Pow(_preferredMixture.SaltyValue - mixture.SaltyValue, 2);
        float x5 = Mathf.Pow(_preferredMixture.UmamiValue - mixture.UmamiValue, 2);
        float distance = Mathf.Sqrt(x1 + x2 + x3 + x4 + x5);

        float ageFactor = 1f + (ageLevel * _ageMultiplier);
        return (int)(_baseSellValue * (1f / (distance + 1f)) * ageFactor);
    }

    public IngredientData GetStats()
    {
        return _preferredMixture;
    }

    public bool Interact()
    {
        Rigidbody heldObject = PlayerController.Instance.PhysicsPickup.GetHeldObject();
        if (heldObject == null)
            return false;

        if (!heldObject.TryGetComponent(out Bottle bottle))
            return false;

        bottle.GetMixture(out IngredientData mixture, out int ageLevel);
        int sellValue = CalculateSellValue(mixture, ageLevel);
        PlayerController.Instance.PlayerWallet.Deposit(sellValue);
        PlayerController.Instance.PhysicsPickup.DropObject();
        Destroy(bottle.gameObject);
        return true;
    }
}
