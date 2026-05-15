using UnityEngine;
using System.Collections.Generic;

public class LaserAbility : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask effectOnlyLayers;
    [SerializeField] private float laserWidth;

    [Header("Impact Effects")]
    [SerializeField] private List<LaserImpactEntry> impactEffects;

    private BoxCollider boxCollider;
    private float currentLaserLength = 0;

    private GameObject currentEffect;
    private int currentHitLayer = -1;
    private float lastEffectSpawnTime = -999f;

    private readonly HashSet<Collider> currentOverlapping = new();

    [System.Serializable]
    public class LaserImpactEntry
    {
        public string label;
        [Layer] public int layer;
        public GameObject prefab;
        public float effectDuration = 1f;
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        currentLaserLength = maxDistance;
    }

    private void LateUpdate()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            currentLaserLength = hitInfo.distance;
            HandleImpactEffect(hitInfo);
        }
        else
        {
            currentLaserLength = maxDistance;

            if (Physics.Raycast(origin, direction, out RaycastHit effectHitInfo, maxDistance, effectOnlyLayers, QueryTriggerInteraction.Collide))
                HandleImpactEffect(effectHitInfo);
            else
                ClearImpactEffect();
        }

        UpdateCollider(currentLaserLength);
        CheckLaserOverlap();

        Debug.DrawRay(origin, direction * currentLaserLength, Color.red);
    }

    private void UpdateCollider(float length)
    {
        boxCollider.center = new Vector3(0, 0, length / 2f);
        boxCollider.size = new Vector3(laserWidth, laserWidth, length);
    }

    private void CheckLaserOverlap()
    {
        Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size / 2f;

        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            halfExtents,
            transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        HashSet<Collider> newOverlapping = new();

        foreach (Collider col in hits)
        {
            if (col.gameObject == gameObject) continue;

            newOverlapping.Add(col);

            if (!currentOverlapping.Contains(col))
            {
                col.SendMessage("OnLaserEnter", gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }

        foreach (Collider col in currentOverlapping)
        {
            if (!newOverlapping.Contains(col))
                col.SendMessage("OnLaserExit", gameObject, SendMessageOptions.DontRequireReceiver);
        }

        currentOverlapping.Clear();
        foreach (var c in newOverlapping) currentOverlapping.Add(c);
    }

    private void HandleImpactEffect(RaycastHit hitInfo)
    {
        int hitLayer = hitInfo.collider.gameObject.layer;
        string hitLayerName = LayerMask.LayerToName(hitLayer);

        LaserImpactEntry entry = GetEntryForLayer(hitLayer);

        if (hitLayer != currentHitLayer)
        {
            ClearImpactEffect();
            currentHitLayer = hitLayer;
        }

        float duration = entry != null ? entry.effectDuration : 1f;
        bool shouldSpawn = currentEffect == null || (Time.time - lastEffectSpawnTime >= duration);

        if (shouldSpawn)
        {
            if (entry?.prefab != null)
            {
                if (currentEffect != null)
                    Destroy(currentEffect);

                currentEffect = Instantiate(entry.prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                lastEffectSpawnTime = Time.time;
                Debug.Log($"[Laser] Impacto con: {hitInfo.collider.name} | Layer: {hitLayerName} | Spawn de: {entry.prefab.name}");
            }
            else if (entry == null)
            {
                Debug.LogWarning($"[Laser] Impacto con: {hitInfo.collider.name} | Layer: {hitLayerName} | Sin prefab asignado");
            }
        }
        else if (currentEffect != null)
        {
            currentEffect.transform.position = hitInfo.point;
            currentEffect.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        }
    }

    private void ClearImpactEffect()
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
        currentHitLayer = -1;
        lastEffectSpawnTime = -999f;
    }

    private LaserImpactEntry GetEntryForLayer(int layer)
    {
        foreach (var entry in impactEffects)
        {
            if (entry.layer == layer) return entry;
        }
        return null;
    }

    private void OnDisable()
    {
        foreach (Collider col in currentOverlapping)
            col.SendMessage("OnLaserExit", gameObject, SendMessageOptions.DontRequireReceiver);

        currentOverlapping.Clear();
        ClearImpactEffect();
    }
}