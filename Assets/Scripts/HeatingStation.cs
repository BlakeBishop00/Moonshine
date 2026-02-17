using UnityEngine;

public class HeatingStation : StationBase
{
    private IFlammable _flammableObject;

    void Start()
    {
        OnStationEquipped.AddListener((brewingPot) =>
        {
            if (brewingPot.TryGetComponent(out IFlammable flammable))
            {
                _flammableObject = flammable;
            }
        });

        OnStationUnequipped.AddListener(() =>
        {
            _flammableObject = null;
        });

        OnStationTick.AddListener(() =>
        {
            if (_flammableObject != null)
            {
                _flammableObject.TickFire();
            }
        });
    }
}
