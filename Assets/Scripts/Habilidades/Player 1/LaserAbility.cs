using UnityEngine;

public class LaserAbility : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask hitLayers;

    private BoxCollider boxCollider;
    private RaycastHit hitInfo;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        currentLaserLength=maxDistance;
    }

    private float currentLaserLength = 0;

    private void LateUpdate()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
            currentLaserLength = hitInfo.distance;
        else
            currentLaserLength = maxDistance;

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("rasho laser impacto con: " + other.name + " | Tag: " + other.tag);
    }
}