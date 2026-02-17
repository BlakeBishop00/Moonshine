using UnityEngine;

public class AgeingStation : StationBase
{
    private IAgeable _ageableObject;

    void Start()
    {
        OnStationEquipped.AddListener((brewingPot) =>
        {
            if (brewingPot.TryGetComponent(out IAgeable ageable))
            {
                _ageableObject = ageable;
            }
        });

        OnStationUnequipped.AddListener(() =>
        {
            _ageableObject = null;
        });

        OnStationTick.AddListener(() =>
        {
            if (_ageableObject != null)
            {
                _ageableObject.TickAge();
            }
        });
    }
}
