using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private float cullRadius = 60f;
    [SerializeField] private float sphereRadius = 1.5f;

    private const int MaxSpheres = 65536;
    private BoundingSphere[] spheres = new BoundingSphere[MaxSpheres];
    private int[] objectIndices = new int[MaxSpheres];

    private CullingGroup cullingGroup;
    private int activeSphereCount = 0;
    private Dictionary<int, int> originalToSphereIndex = new Dictionary<int, int>();

    private IEnumerator InitializeCullingAsync()
    {
        int total = objectsToCull.Length;
        int processed = 0;
        const int batchSize = 200;

        for (int i = 0; i < total; i++)
        {
            var obj = objectsToCull[i];
            if (obj == null) continue;

            spheres[activeSphereCount] = new BoundingSphere(obj.transform.position, sphereRadius);
            objectIndices[activeSphereCount] = i;
            originalToSphereIndex[i] = activeSphereCount;
            activeSphereCount++;

            processed++;
            if (processed % batchSize == 0)
            {
                yield return null;
            }
        }

        cullingGroup = new CullingGroup();
        cullingGroup.targetCamera = Camera.main;
        cullingGroup.SetBoundingSpheres(spheres);
        cullingGroup.SetBoundingSphereCount(activeSphereCount);
        cullingGroup.SetBoundingDistances(new float[] { cullRadius });
        cullingGroup.SetDistanceReferencePoint(player);
        cullingGroup.onStateChanged = OnCullingStateChanged;
    }

    private void OnCullingStateChanged(CullingGroupEvent evt)
    {
        if (evt.index >= activeSphereCount) return;

        bool shouldBeActive = evt.currentDistance == 0;
        int objIdx = objectIndices[evt.index];
        var go = objectsToCull[objIdx];

        if (go != null && go.activeSelf != shouldBeActive)
        {
            go.SetActive(shouldBeActive);
        }
    }

    public void UpdateObjectPosition(int originalArrayIndex, Vector3 newPos)
    {
        if (!originalToSphereIndex.TryGetValue(originalArrayIndex, out int sphereIdx))
        {
            return;
        }

        spheres[sphereIdx].position = newPos;
    }

    private void OnDestroy()
    {
        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
        originalToSphereIndex.Clear();
    }

    public Transform player;
    public GameObject[] objectsToCull;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        StartCoroutine(InitializeCullingAsync());
    }
}
