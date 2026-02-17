using UnityEngine;

public class BrewingPot : MonoBehaviour, IAgeable, IFlammable, IMixable, IInteractable
{
    private Renderer _renderer;
    private IngredientData _currentMixture;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        ResetPot();
    }

    public void ResetPot()
    {
        _currentMixture = ScriptableObject.CreateInstance<IngredientData>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out IngredientBase ingredient))
            return;
        
        // AddIngredient(ingredient.IngredientData);
        // Destroy(collision.gameObject);
    }

    private void AddIngredient(IngredientData ingredient)
    {
        Debug.Log("Added ingredient: " + ingredient.name);

        _currentMixture.WaterValue += ingredient.WaterValue;
        _currentMixture.YeastValue += ingredient.YeastValue;
        _currentMixture.SweetValue += ingredient.SweetValue;
        _currentMixture.SourValue += ingredient.SourValue; 
        _currentMixture.BitterValue += ingredient.BitterValue;
        _currentMixture.SaltyValue += ingredient.SaltyValue;
        _currentMixture.UmamiValue += ingredient.UmamiValue;
    }

    public void TickAge()
    {
        Debug.Log("Aging mixture");

        // Temporary Visual feedback
        _renderer.material.color = Color.Lerp(_renderer.material.color, Color.black, 0.1f);
    }

    public void TickFire()
    {
        Debug.Log("Heating mixture");

        // Temporary Visual feedback
        _renderer.material.color = Color.Lerp(_renderer.material.color, Color.red, 0.1f);
    }

    public void TickMix()
    {
        Debug.Log("Mixing mixture");

        // Temporary Visual feedback
        _renderer.material.color = Color.Lerp(_renderer.material.color, Color.green, 0.1f);
    }

    public bool Interact()
    {
        Debug.Log("Interacting with brewing pot");
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
}
