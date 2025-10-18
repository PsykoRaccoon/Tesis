using UnityEngine;

public class LaserAbility : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask hitLayers;

    private BoxCollider boxCollider;
    private RaycastHit hitInfo;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hitInfo, maxDistance, hitLayers))
        {
            UpdateCollider(hitInfo.distance);
        }
        else
        {
            UpdateCollider(maxDistance);
        }

        Debug.DrawRay(origin, direction * (hitInfo.collider ? hitInfo.distance : maxDistance), Color.red);
    }

    private void UpdateCollider(float length)
    {
        boxCollider.center = new Vector3(0, 0, length / 2f);
        boxCollider.size = new Vector3(0.5f, 0.5f, length);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Impacto con {other.name}");
    }
}
