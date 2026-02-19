using UnityEngine;

public class DistillCollector : MonoBehaviour, IInteractable
{
    [SerializeField] private int _ageMultiplier = 2;
    private IngredientData _currentMixture;
    private int _waterLevel = 0;
    private int _ageLevel = -1;

    void Awake()
    {
        ResetCollector();
    }

    public bool Interact()
    {
        return CollectDistill();
    }

    public bool CollectDistill()
    {
        Rigidbody heldObject = PlayerController.Instance.PhysicsPickup.GetHeldObject();

        if (heldObject == null)
            return false;

        if (!heldObject.TryGetComponent(out Bottle bottle))
            return false;

        if (_waterLevel <= 0)
        {
            Debug.Log("Not enough water to bottle");
            return false;
        }

        if (_ageLevel <= 0)
        {
            Debug.Log("Not aged enough to bottle");
            return false;
        }

        _waterLevel--;

        if (_waterLevel == 0)
        {
            ResetCollector();
        }
        
        bottle.SetMixture(_currentMixture, _ageLevel);
        Debug.Log("Transferred mixture to bottle");
        return true;
    }

    private void ResetCollector()
    {
        _currentMixture = null;
        _waterLevel = 0;
        _ageLevel = -1;
    }

    public bool DepositMixture(IngredientData mixture, int ageLevel)
    {
        if (_currentMixture != null && _currentMixture != mixture)
        {
            Debug.Log("Already has mixture, cannot deposit");
            return false;
        }

        int multipliedAge = ageLevel * _ageMultiplier;

        if (_ageLevel != -1 && _ageLevel != multipliedAge)
        {
            Debug.Log("Mismatch age level, cannot deposit");
            return false;
        }

        _currentMixture = mixture;
        _waterLevel++;
        _ageLevel = multipliedAge;
        return true;
    }
}
