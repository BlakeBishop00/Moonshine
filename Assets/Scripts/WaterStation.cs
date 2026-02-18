using UnityEngine;

public class WaterStation : StationBase
{
    private IWaterable _waterableObject;

    void Start()
    {
        OnStationEquipped.AddListener((brewingPot) =>
        {
            if (brewingPot.TryGetComponent(out IWaterable waterable))
            {
                _waterableObject = waterable;
            }
        });

        OnStationUnequipped.AddListener(() =>
        {
            _waterableObject = null;
        });

        OnStationTick.AddListener(() =>
        {
            if (_waterableObject != null)
            {
                _waterableObject.TickWater();
            }
        });
    }
}
