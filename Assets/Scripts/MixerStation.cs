using UnityEngine;

public class MixerStation : StationBase
{
    [SerializeField] private Vector3 _rotationSpeed = Vector3.zero;
    private IMixable _mixableObject;

    void Start()
    {
        OnStationEquipped.AddListener((brewingPot) =>
        {
            if (brewingPot.TryGetComponent(out IMixable mixable))
            {
                _mixableObject = mixable;
            }
        });

        OnStationUnequipped.AddListener(() =>
        {
            _mixableObject = null;
        });

        OnStationTick.AddListener(() =>
        {
            if (_mixableObject != null)
            {
                _mixableObject.TickMix();
            }
        });
    }

    void FixedUpdate()
    {
        if (!_isActive || _equippedBrewingPot == null)
            return;

        Quaternion newRotation = Quaternion.Euler(_rotationSpeed * Time.deltaTime);
        _equippedBrewingPot.MoveRotation(_equippedBrewingPot.rotation * newRotation);
    }
}
