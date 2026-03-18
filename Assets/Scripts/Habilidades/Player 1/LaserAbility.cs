using UnityEngine;

public class LaserDynamic : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask hitLayers;

    [Header("Refs")]
    [SerializeField] private Transform visual;
    [SerializeField] private BoxCollider boxCollider;

    private RaycastHit hitInfo;

    private void Update()
    {
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        float distance = maxDistance;

        if (Physics.Raycast(origin, direction, out hitInfo, maxDistance, hitLayers))
        {
            distance = hitInfo.distance;
        }

        UpdateVisual(distance);
        UpdateCollider(distance);

        Debug.DrawRay(origin, direction * distance, Color.red);
    }

    private void UpdateVisual(float length)
    {
        visual.localScale = new Vector3(
            visual.localScale.x,
            visual.localScale.y,
            length
        );

        visual.localPosition = new Vector3(0, 0, length / 2f);
    }

    private void UpdateCollider(float length)
    {
        boxCollider.size = new Vector3(0.5f, 0.5f, length);
        boxCollider.center = new Vector3(0, 0, length / 2f);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Golpeando {other.name}");
    }
}