using UnityEngine;
using System.Collections.Generic;

public class LaserAbility : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask effectOnlyLayers;

    [Header("Impact Effects")]
    [SerializeField] private List<LaserImpactEntry> impactEffects;
    

    private BoxCollider boxCollider;
    private float currentLaserLength = 0;

    private GameObject currentEffect;
    private int currentHitLayer = -1;

    [System.Serializable]
    public class LaserImpactEntry
    {
        public string label;           
        [Layer] public int layer;      
        public GameObject prefab;     
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
            {
                HandleImpactEffect(effectHitInfo);
            }
            else
            {
                ClearImpactEffect();
            }
        }

        Debug.DrawRay(origin, direction * currentLaserLength, Color.red);
    }

    private void FixedUpdate()
    {
        UpdateCollider(currentLaserLength);
    }

    private void UpdateCollider(float length)
    {
        boxCollider.center = new Vector3(0, 0, length / 2f);
        boxCollider.size = new Vector3(0.5f, 0.5f, length);
    }

    private void HandleImpactEffect(RaycastHit hitInfo)
    {
        int hitLayer = hitInfo.collider.gameObject.layer;
        string hitLayerName = LayerMask.LayerToName(hitLayer);

        if (hitLayer != currentHitLayer)
        {
            ClearImpactEffect();
            currentHitLayer = hitLayer;

            GameObject prefab = GetPrefabForLayer(hitLayer);
            if (prefab != null)
            {
                currentEffect = Instantiate(prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Debug.Log($"[Laser] Impacto con: {hitInfo.collider.name} | Layer: {hitLayerName} | Prefab instanciado: {prefab.name}");
            }
            else
            {
                Debug.LogWarning($"[Laser] Impacto con: {hitInfo.collider.name} | Layer: {hitLayerName} | Sin prefab asignado para este layer");
            }
        }
    }

    private void ClearImpactEffect()
    {
        if (currentEffect != null)
        {
            Debug.Log($"[Laser] Efecto limpiado: {currentEffect.name}");
            Destroy(currentEffect);
            currentEffect = null;
        }
        currentHitLayer = -1;
    }

    private GameObject GetPrefabForLayer(int layer)
    {
        foreach (var entry in impactEffects)
        {
            if (entry.layer == layer)
                return entry.prefab;
        }
        return null;
    }

    private void OnDisable()
    {
        ClearImpactEffect();
    }
}

