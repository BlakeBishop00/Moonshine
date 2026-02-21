using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsPickup : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Rigidbody> OnPickup;
    [HideInInspector] public UnityEvent<Rigidbody> OnDrop;

    [Header("Interact Settings")]
    [SerializeField] private LayerMask _interactLayer;
    [Header("Pickup Settings")]
    [SerializeField] private LayerMask _pickupLayer;
    [SerializeField] private float _pickupRange = 6f;
    [SerializeField] private float _holdDistance = 2f;
    [SerializeField] private float _moveForce = 500f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _throwForce = 10f;
    [SerializeField] private float _linearDampening = 10f;

    private List<Collider> _colliders = new List<Collider>();
    private Camera _playerCamera;
    private PlayerInputActions _inputActions;
    private Rigidbody _heldObject;
    private float _originalLinearDamping;

    void Awake()
    {
        _playerCamera = Camera.main;
        _inputActions = new PlayerInputActions();
        _inputActions.Player.Enable();

        _inputActions.Player.Interact.performed += ctx => HandleInteract();
        _inputActions.Player.Attack.performed += ctx => HandleThrow();
    }

    void FixedUpdate()
    {
        if (_heldObject)
            MoveObject();
    }

    public void DropObject()
    {
        _heldObject.useGravity = true;
        _heldObject.linearDamping = _originalLinearDamping;
        _colliders.ForEach(c => c.enabled = true);
        OnDrop.Invoke(_heldObject);
        _heldObject = null;
        FMODUnity.RuntimeManager.PlayOneShot("event:/player/actions/ItemPlace");
    }

    public Rigidbody GetHeldObject() => _heldObject;

    void HandleInteract()
    {
        if (_heldObject == null)
        {
            if (TryPickup())
                return;

            TryInteract();
            return;
        }

        if (TryInteract())
            return;

        DropObject();
    }

    void HandleThrow()
    {
        if (_heldObject)
            ThrowObject();
    }

    bool TryPickup()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, _pickupRange, _pickupLayer))
            return false;
        
        if (!hit.collider.TryGetComponent(out Rigidbody rb))
            return false;

        _heldObject = rb;
        _originalLinearDamping = _heldObject.linearDamping;
        _heldObject.linearDamping = _linearDampening;
        _heldObject.useGravity = false;
        OnPickup.Invoke(_heldObject);

        _colliders = _heldObject.GetComponentsInChildren<Collider>().ToList();
        _colliders.ForEach(c => c.enabled = false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/player/actions/ItemPickup");

        return true;
    }

    bool TryInteract()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, _pickupRange, _interactLayer))
            return false;
        
        if (!hit.collider.TryGetComponent(out IInteractable interactable))
            return false;

        bool success = interactable.Interact();
        return success;
    }

    void MoveObject()
    {
        Vector3 holdPosition = _playerCamera.transform.position + _playerCamera.transform.forward * _holdDistance;

        Vector3 direction = holdPosition - _heldObject.position;

        _heldObject.linearVelocity = direction * _moveForce * Time.fixedDeltaTime;

        Quaternion targetRotation = _playerCamera.transform.rotation;
        Quaternion newRotation = Quaternion.Slerp(_heldObject.rotation, targetRotation, _rotateSpeed * Time.fixedDeltaTime);
        _heldObject.MoveRotation(newRotation);
    }

    void ThrowObject()
    {
        _heldObject.useGravity = true;
        _heldObject.linearDamping = _originalLinearDamping;
        _heldObject.AddForce(_playerCamera.transform.forward * _throwForce, ForceMode.Impulse);
        _colliders.ForEach(c => c.enabled = true);
        _heldObject = null;
    }
}
