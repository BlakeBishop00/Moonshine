using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PlayerInspector : MonoBehaviour
{
    [HideInInspector] public UnityEvent<IInspectable> OnInspect;
    [SerializeField] private LayerMask _inspectLayer;
    [SerializeField] private float _inspectRange = 6f;
    private Camera _playerCamera;
    private IInspectable _currentInspectable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, _inspectRange, _inspectLayer))
        {
            UpdateInspectable(null);
            return;
        }

        if (!hit.collider.TryGetComponent(out IInspectable inspectable))
        {
            UpdateInspectable(null);
            return;
        }

        UpdateInspectable(inspectable);
    }

    void UpdateInspectable(IInspectable newInspectable)
    {
        _currentInspectable = newInspectable;
        OnInspect?.Invoke(_currentInspectable);
    }
}
