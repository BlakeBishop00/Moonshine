using UnityEngine;
using UnityEngine.Events;

public class StationBase : MonoBehaviour, IInteractable
{
    [HideInInspector] public UnityEvent<BrewingPot> OnStationEquipped;
    [HideInInspector] public UnityEvent OnStationUnequipped;
    [HideInInspector] public UnityEvent OnStationTick;

    [Header("Station Settings")]
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    [SerializeField] private float _tickRate = 60f;
    protected Rigidbody _equippedBrewingPot;
    protected bool _isActive = false;
    private float _tickTimer = 0f;

    void Update()
    {
        if (!_isActive)
        {
            _tickTimer = 0f;
            return;
        }

        _tickTimer += Time.deltaTime;

        // Convert tick rate to seconds and check if its time to tick
        float timeTillTick = 1f / _tickRate;

        if (_tickTimer < timeTillTick)
            return;
        
        _tickTimer = 0f;

        OnStationTick.Invoke();
    }

    public virtual bool Interact()
    {
        if (HandlePlacement())
            return true;
        
        _isActive = !_isActive;
        return true;
    }

    private bool HandlePlacement()
    {
        Rigidbody heldObject = PlayerController.Instance.PhysicsPickup.GetHeldObject();
        
        if (_equippedBrewingPot != null)
            return false;

        if (heldObject == null)
            return false;

        if (!heldObject.TryGetComponent(out BrewingPot brewingPot))
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
