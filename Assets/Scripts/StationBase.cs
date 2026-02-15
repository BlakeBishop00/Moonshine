using UnityEngine;
using UnityEngine.Events;

public class StationBase : MonoBehaviour
{
    [HideInInspector] public UnityEvent<BrewingPot> OnStationEquipped;
    [HideInInspector] public UnityEvent OnStationUnequipped;
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    private BrewingPot _equippedBrewingPot;


    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out BrewingPot brewingPot))
            return;
        
        _equippedBrewingPot = brewingPot;

        Rigidbody rb = brewingPot.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;

        brewingPot.transform.position = transform.position + _positionOffset;
        brewingPot.transform.rotation = transform.rotation;

        OnStationEquipped.Invoke(brewingPot);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BrewingPot brewingPot) && brewingPot == _equippedBrewingPot)
        {
            OnStationUnequipped.Invoke();
            _equippedBrewingPot = null;
        }
    }
}
