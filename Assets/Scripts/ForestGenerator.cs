using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns trees so that their pivots are inside the BoxCollider attached
/// to the same GameObject. Uses Poisson-disc sampling for ≥ minDistance
/// spacing. Works in the Box's local space, so rotation & non-uniform
/// scale are handled automatically.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class ForestGenerator : MonoBehaviour
{
    [Header("Prefabs (drag at least one):")]
    [SerializeField] List<GameObject> treePrefabs = new();

    [Header("Sampling")]
    [SerializeField, Min(1)] int desiredTreeCount = 100;
    [SerializeField, Min(0.1f)] float minDistance = 3f;  // metres

    [Header("Vertical placement (position offset)")]
    [SerializeField] Vector2 yOffsetRange = new(4.7f, 5.2f);  // metres
    [Tooltip("Extra lift to compensate for a pivot that sits at the tree's centre.")]
    [SerializeField] float pivotLift = 0f;  // set ≈ prefabHeight / 2 if needed

    [Header("Exlusion Controls")]
    [SerializeField] List<Transform> exclusionPoints = new();
    [SerializeField] LayerMask exclusionLayers = 1 << 31;

    [Header("Debug")]
    [SerializeField] bool drawGizmos = false;

    Transform forestParent;          // container for hierarchy tidiness
    BoxCollider area;

    void Start()
    {
        if (treePrefabs.Count == 0) { Debug.LogError("ForestGenerator: No prefabs!"); return; }

        area = GetComponent<BoxCollider>();
        forestParent = GameObject.Find("/FOREST")?.transform             // reuse if it exists
                ?? new GameObject("FOREST").transform;            // or make one
        forestParent.SetParent(null);

        // cache exclusion world-space positions
        Vector3[] excl = new Vector3[exclusionPoints.Count];
        for (int i = 0; i < excl.Length; i++) excl[i] = exclusionPoints[i].position;

        Vector3 half = area.size * 0.5f;          // half-extents in *local* space
        Vector2 dims = new(half.x * 2, half.z * 2);

        // -----------------------------------------------------------
        // 1. Poisson-disc sample points in XZ plane (local space)
        // -----------------------------------------------------------
        List<Vector2> samples = PoissonDisc2D(dims, minDistance, desiredTreeCount);

        // -----------------------------------------------------------
        // 2. Spawn each tree
        // -----------------------------------------------------------
        foreach (Vector2 p in samples)
        {
            // Convert (local XZ) => local position centred on collider
            Vector3 localPos = new(
                p.x - dims.x * 0.5f + area.center.x,
                area.center.y,
                p.y - dims.y * 0.5f + area.center.z);

            // Local → world
            Vector3 worldPos = transform.TransformPoint(localPos);

            // Apply vertical offsets
            worldPos += Vector3.up * (pivotLift + Random.Range(yOffsetRange.x, yOffsetRange.y));

            // skip if too close to a manual exclusion point
            bool tooClose = false;
            foreach (Vector3 e in excl)
                if ((e - worldPos).sqrMagnitude < minDistance * minDistance)
                { tooClose = true; break; }
            if (tooClose) continue;

            // skip if overlapping a collider on exclusionLayers
            if (exclusionLayers.value != 0 &&
                Physics.CheckSphere(worldPos, minDistance * 0.5f, exclusionLayers))
                continue;

            // Pick a prefab & instantiate
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Count)];
            GameObject t = Instantiate(prefab, worldPos,
                                       Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                                       forestParent);
        }
    }

    // -----------------------------------------------------------------
    // Bridson Poisson-disc in 2D rectangle (dims.x × dims.y)
    // -----------------------------------------------------------------
    static List<Vector2> PoissonDisc2D(Vector2 dims, float r, int max)
    {
        float cell = r / Mathf.Sqrt(2);
        int cols = Mathf.CeilToInt(dims.x / cell);
        int rows = Mathf.CeilToInt(dims.y / cell);

        Vector2[,] grid = new Vector2[cols, rows];
        List<Vector2> active = new();
        List<Vector2> result = new();

        Vector2 first = new(Random.value * dims.x, Random.value * dims.y);
        grid[(int)(first.x / cell), (int)(first.y / cell)] = first;
        active.Add(first); result.Add(first);

        while (active.Count > 0 && result.Count < max)
        {
            int idx = Random.Range(0, active.Count);
            Vector2 point = active[idx];
            bool found = false;

            for (int k = 0; k < 30; k++)
            {
                float theta = Random.value * Mathf.PI * 2;
                float mag = Random.Range(r, 2f * r);
                Vector2 sample = point + new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * mag;

                if (sample.x < 0 || sample.x >= dims.x || sample.y < 0 || sample.y >= dims.y)
                    continue;

                int gx = (int)(sample.x / cell);
                int gy = (int)(sample.y / cell);

                bool ok = true;
                for (int i = Mathf.Max(0, gx - 2); i <= Mathf.Min(gx + 2, cols - 1); i++)
                    for (int j = Mathf.Max(0, gy - 2); j <= Mathf.Min(gy + 2, rows - 1); j++)
                        if (grid[i, j] != Vector2.zero &&
                            (grid[i, j] - sample).sqrMagnitude < r * r)
                        { ok = false; break; }

                if (ok)
                {
                    grid[gx, gy] = sample;
                    active.Add(sample);
                    result.Add(sample);
                    found = true; break;
                }
            }
            if (!found) active.RemoveAt(idx);
        }
        return result;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || forestParent == null) return;
        Gizmos.color = Color.green;
        foreach (Transform t in forestParent)
            Gizmos.DrawWireSphere(t.position + Vector3.up * 0.1f, 0.25f);
    }
#endif
}