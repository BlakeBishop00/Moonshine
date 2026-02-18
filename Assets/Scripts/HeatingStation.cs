using UnityEngine;

public class HeatingStation : StationBase
{
    [SerializeField] private Renderer _heatingEffectRenderer;
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

        OnStationActivate.AddListener(() =>
        {
            _heatingEffectRenderer.material.color = Color.red;
        });

        OnStationDeactivate.AddListener(() =>
        {
            _heatingEffectRenderer.material.color = Color.gray;
        });

        _heatingEffectRenderer.material.color = Color.gray;
    }
}
