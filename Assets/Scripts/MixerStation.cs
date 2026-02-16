using UnityEditor.Callbacks;
using UnityEngine;

public class MixerStation : StationBase
{
    [SerializeField] private float _mixingTickRate = 60f;
    [SerializeField] private Vector3 _rotationSpeed = Vector3.zero;
    private IMixable _mixableObject;
    private float _tickTimer;
    private bool _isMixing;

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
        if (!_isMixing)
        {
            _tickTimer = 0f;
            return;
        }

        _tickTimer += Time.deltaTime;

        float mixingTime = _mixingTickRate / 60f;

        if (_tickTimer < mixingTime)
            return;
        
        _tickTimer = 0f;

        if (_mixableObject != null)
        {
            _mixableObject.TickMix();
        }
    }

    void FixedUpdate()
    {
        if (!_isMixing || _equippedBrewingPot == null)
            return;

        Quaternion newRotation = Quaternion.Euler(_rotationSpeed * Time.deltaTime);
        _equippedBrewingPot.MoveRotation(_equippedBrewingPot.rotation * newRotation);
    }

    public override bool Interact()
    {
        Debug.Log("Interact with mixer station");
        if (base.Interact())
            return true;

        _isMixing = !_isMixing;
        Debug.Log("Mixing: " + _isMixing);
        return true;
    }
}
