using System;
using UnityEngine;

public class BillboardSpriteManage : MonoBehaviour
{
    public bool rotate = true;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private bool useMirroring = true;
    [SerializeField] private float angleStep = 45f;
    [SerializeField] private bool backIsFront = false;
    [SerializeField] private Transform facingTransform;

    MeshSpriteRenderer meshSprite;
    Camera mainCamera;
    Vector3 initialForward;



    int lastIndex = -1;
    bool lastFlip;


    void Awake()
    {
        meshSprite = GetComponent<MeshSpriteRenderer>();

        mainCamera = Camera.main;

        if (facingTransform == null)
            initialForward = transform.forward;
        

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("Sprites array is empty!", this);
            enabled = false;
        }
    }

    void Start()
    {
        meshSprite.ApplySprite(sprites[0], flipX: false); // Start with a sprite.
    }

    void Update()
    {
        if (sprites.Length > 1)
        {

            if (mainCamera == null) return;

            Vector3 forward = facingTransform != null
                ? facingTransform.forward
                : initialForward;

            Vector3 toCamera = mainCamera.transform.position - transform.position;
            toCamera.y = 0f;
            toCamera.Normalize();

            Vector3 projectedForward = forward;
            projectedForward.y = 0f;
            projectedForward.Normalize();

            float signedAngle = Vector3.SignedAngle(
                projectedForward,
                toCamera,
                Vector3.up
            );

            int index;
            bool flipX = false;

            if (useMirroring)
            {
                bool shouldMirror = signedAngle < 0f;
                float absAngle = Mathf.Abs(signedAngle);

                int numSteps = Mathf.RoundToInt(180f / angleStep);
                index = Mathf.RoundToInt(absAngle / angleStep);
                index = Mathf.Clamp(index, 0, numSteps);

                if (backIsFront && index == numSteps)
                    index = 0;

                index = Mathf.Clamp(index, 0, sprites.Length - 1);

                bool isFront = index == 0;
                bool isBack = (index == sprites.Length - 1) && !backIsFront;

                flipX = shouldMirror && !isFront && !isBack;
            }
            else
            {
                float normalizedAngle = (signedAngle + 360f) % 360f;
                float step = 360f / sprites.Length;
                index = Mathf.RoundToInt(normalizedAngle / step) % sprites.Length;
            }

            // Only update renderer if something actually changed
            if (index != lastIndex || flipX != lastFlip)
            {
                meshSprite.ApplySprite(sprites[index], flipX);
                lastIndex = index;
                lastFlip = flipX;
            }
        }
    }

    void LateUpdate()
    {
        if (rotate)
        {
            if (mainCamera == null) return;

            Vector3 cameraDirection = mainCamera.transform.forward;
            cameraDirection.y = 0f;

            transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
        }
    }
}
