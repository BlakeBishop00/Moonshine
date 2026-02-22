using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed = Vector3.zero;

    void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}
