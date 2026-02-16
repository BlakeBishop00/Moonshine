using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public PhysicsPickup PhysicsPickup { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        PhysicsPickup = GetComponent<PhysicsPickup>();
        PlayerMovement = GetComponent<PlayerMovement>();
    }
}
