using UnityEngine;

public class AgeingStation : StationBase
{
    [SerializeField] private float _ageingTickRate = 60f;
    private IAgeable _ageableObject;
    private float _tickTimer;

    void Start()
    {
        _tickTimer = 0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        _tickTimer += Time.deltaTime;

        float ageingTime = _ageingTickRate / 60f;

        if (_tickTimer < ageingTime)
            return;
        
        _tickTimer = 0f;

        if (_ageableObject != null)
            _ageableObject.TickAge();
    }
}
