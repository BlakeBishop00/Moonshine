using UnityEngine;

public class MixerStation : StationBase
{
    [SerializeField] private float _mixingTickRate = 60f;
    private IMixable _mixableObject;
    private float _tickTimer;

    void Start()
    {
        _tickTimer = 0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        _tickTimer += Time.deltaTime;

        float mixingTime = _mixingTickRate / 60f;

        if (_tickTimer < mixingTime)
            return;
        
        _tickTimer = 0f;

        if (_mixableObject != null)
            _mixableObject.TickMix();
    }
}
