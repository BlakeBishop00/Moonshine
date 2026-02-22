using UnityEngine;
using UnityEngine.UI;

public class AgeingStation : StationBase
{
    [SerializeField] private Renderer _ageingEffectRenderer;
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

        OnStationActivate.AddListener(() =>
        {
            _ageingEffectRenderer.enabled = true;
        });

        OnStationDeactivate.AddListener(() =>
        {
            _ageingEffectRenderer.enabled = false;
        });
    }
}
