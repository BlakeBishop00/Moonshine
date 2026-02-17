using System.Collections.Generic;
using UnityEngine;

public class BrewingPot : MonoBehaviour, IAgeable, IFlammable, IMixable, IInteractable
{
    [SerializeField] private Vector3 _ingredientOffsetMin = Vector3.zero;
    [SerializeField] private Vector3 _ingredientOffsetMax = Vector3.zero;
    [SerializeField] private float _bobRange = 0.01f;
    [SerializeField] private float _bobSpeed = 1f;

    private IngredientData _currentMixture;
    private List<IngredientBase> _attachedIngredients = new List<IngredientBase>();
    private Dictionary<IngredientBase, Vector3> _ingredientPositionsLookup = new Dictionary<IngredientBase, Vector3>();

    void Start()
    {
        ResetPot();
    }

    void Update()
    {
        BobIngredients();
    }

    private void BobIngredients()
    {
        for (int i = 0; i < _attachedIngredients.Count; i++)
        {
            IngredientBase ingredient = _attachedIngredients[i];
            Vector3 originalPosition = _ingredientPositionsLookup[ingredient];
            ingredient.transform.localPosition = originalPosition + Vector3.up * Mathf.Sin(Time.time * _bobSpeed + i) * _bobRange;
            ingredient.gameObject.transform.rotation = transform.rotation;
        }
    }

    public void ResetPot()
    {
        _currentMixture = ScriptableObject.CreateInstance<IngredientData>();
    }

    private void AddIngredient(IngredientData ingredient)
    {
        Debug.Log("Added ingredient: " + ingredient.name);

        _currentMixture += ingredient;
    }

    public void TickAge()
    {
        Debug.Log("Aging mixture");

    }

    public void TickFire()
    {
        Debug.Log("Heating mixture");

    }

    public void TickMix()
    {
        Debug.Log("Mixing mixture");

        if (_attachedIngredients.Count == 0)
            return;
            
        IngredientBase randomIngredient = _attachedIngredients[Random.Range(0, _attachedIngredients.Count)];
        AddIngredient(randomIngredient.IngredientData);
        _attachedIngredients.Remove(randomIngredient);
        _ingredientPositionsLookup.Remove(randomIngredient);
        Destroy(randomIngredient.gameObject);
    }

    public bool Interact()
    {
        if (HandleBottling())
            return true;

        if (HandleIngredientDeposit())
            return true;

        return false;
    }

    private bool HandleBottling()
    {
        Rigidbody heldObject = PlayerController.Instance.PhysicsPickup.GetHeldObject();

        if (heldObject == null)
            return false;

        if (!heldObject.TryGetComponent(out Bottle bottle))
            return false;

        // TODO: Add logic for transfering mixture to bottle i.e. should this mixture be bottleable? should we only transfer a portion of the mixture? etc.
        bottle.SetMixture(_currentMixture);
        Debug.Log("Transferred mixture to bottle");
        return true;
    }

    private bool HandleIngredientDeposit()
    {
        Rigidbody heldObject = PlayerController.Instance.PhysicsPickup.GetHeldObject();

        if (heldObject == null)
            return false;

        if (!heldObject.TryGetComponent(out IngredientBase ingredient))
            return false;

        PlayerController.Instance.PhysicsPickup.DropObject();
        
        ingredient.GetComponent<Rigidbody>().isKinematic = true;
        ingredient.GetComponent<Collider>().enabled = false;
        ingredient.gameObject.transform.SetParent(transform);
        Vector3 randomOffset = new Vector3(
            Random.Range(_ingredientOffsetMin.x, _ingredientOffsetMax.x),
            Random.Range(_ingredientOffsetMin.y, _ingredientOffsetMax.y),
            Random.Range(_ingredientOffsetMin.z, _ingredientOffsetMax.z)
        );
        ingredient.gameObject.transform.rotation = Quaternion.identity;
        ingredient.gameObject.transform.localPosition = randomOffset;

        _attachedIngredients.Add(ingredient);
        _ingredientPositionsLookup[ingredient] = randomOffset;

        return true;
    }
}
