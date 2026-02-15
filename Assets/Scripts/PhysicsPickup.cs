using UnityEngine;

public class PhysicsPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float _pickupRange = 6f;
    [SerializeField] private float _holdDistance = 2f;
    [SerializeField] private float _moveForce = 500f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _throwForce = 10f;
    [SerializeField] private float _linearDampening = 10f;

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

    void HandleInteract()
    {
        if (_heldObject == null)
            TryPickup();
        else
            DropObject();
    }

    void HandleThrow()
    {
        if (_heldObject)
            ThrowObject();
    }

    void TryPickup()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, _pickupRange, pickupLayer))
            return;
        
        if (!hit.collider.TryGetComponent(out Rigidbody rb))
            return;

        _heldObject = rb;
        _originalLinearDamping = _heldObject.linearDamping;
        _heldObject.linearDamping = _linearDampening;
        _heldObject.useGravity = false;
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

    void DropObject()
    {
        _heldObject.useGravity = true;
        _heldObject.linearDamping = _originalLinearDamping;
        _heldObject = null;
    }

    void ThrowObject()
    {
        _heldObject.useGravity = true;
        _heldObject.linearDamping = _originalLinearDamping;
        _heldObject.AddForce(_playerCamera.transform.forward * _throwForce, ForceMode.Impulse);
        _heldObject = null;
    }
}
