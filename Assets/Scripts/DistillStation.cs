using UnityEngine;

public class DistillStation : StationBase
{
    [SerializeField] private Renderer _heatingEffectRenderer;
    private IFlammable _iflammableObject;

    void Start()
    {
        OnStationEquipped.AddListener((brewingPot) =>
        {
            if (brewingPot.TryGetComponent(out IFlammable iflammableObject))
            {
                _iflammableObject = iflammableObject;
            }
        });

        OnStationUnequipped.AddListener(() =>
        {
            _iflammableObject = null;
        });

        OnStationTick.AddListener(() =>
        {
            if (_iflammableObject != null)
            {
                _iflammableObject.TickFire();
            }
        });

        OnStationActivate.AddListener(() =>
        {
            _heatingEffectRenderer.material.color = Color.red;
            FMODUnity.RuntimeManager.PlayOneShot("event:/AffectedByReverb/actions/Distilling");
        });

        OnStationDeactivate.AddListener(() =>
        {
            _heatingEffectRenderer.material.color = Color.gray;
        });

        _heatingEffectRenderer.material.color = Color.gray;
    }
}
