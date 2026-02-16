using UnityEngine;
using UnityEngine.Events;

public class StationBase : MonoBehaviour, IInteractable
{
    [HideInInspector] public UnityEvent<BrewingPot> OnStationEquipped;
    [HideInInspector] public UnityEvent OnStationUnequipped;
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    protected Rigidbody _equippedBrewingPot;

    public virtual bool Interact()
    {
        Rigidbody heldObject = PlayerController.Instance.PhysicsPickup.GetHeldObject();
        if (heldObject == null)
            return false;

        if (!heldObject.TryGetComponent(out BrewingPot brewingPot))
            return false;

        if (_equippedBrewingPot != null)
            return false;

        PlayerController.Instance.PhysicsPickup.DropObject();
        _equippedBrewingPot = heldObject;
        _equippedBrewingPot.isKinematic = true;
        _equippedBrewingPot.transform.position = transform.position + _positionOffset;
        _equippedBrewingPot.transform.rotation = transform.rotation;

        OnStationEquipped.Invoke(brewingPot);

        PlayerController.Instance.PhysicsPickup.OnPickup.AddListener(OnPlayerPickup);
        return true;
    }

    private void OnPlayerPickup(Rigidbody pickedUpObject)
    {
        if (pickedUpObject == _equippedBrewingPot)
        {
            _equippedBrewingPot.isKinematic = false;
            _equippedBrewingPot = null;
            OnStationUnequipped.Invoke();
            PlayerController.Instance.PhysicsPickup.OnPickup.RemoveListener(OnPlayerPickup);
        }
    }
}
