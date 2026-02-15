using UnityEngine;

public class HeatingStation : StationBase
{
    [SerializeField] private float _heatingTickRate = 60f;
    private IFlammable _heatableObject;
    private float _tickTimer;

    void Start()
    {
        _tickTimer = 0f;

        OnStationEquipped.AddListener((brewingPot) =>
        {
            if (brewingPot.TryGetComponent(out IFlammable flammable))
            {
                _heatableObject = flammable;
            }
        });

        OnStationUnequipped.AddListener(() =>
        {
            _heatableObject = null;
        });
    }

    // Update is called once per frame
    void Update()
    {
        _tickTimer += Time.deltaTime;

        float heatingTime = _heatingTickRate / 60f;

        if (_tickTimer < heatingTime)
            return;
        
        _tickTimer = 0f;

        if (_heatableObject != null)
            _heatableObject.TickFire();
    }
}
