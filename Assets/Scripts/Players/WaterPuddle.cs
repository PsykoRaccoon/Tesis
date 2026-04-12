using UnityEngine;

public class WaterPuddle : MonoBehaviour
{
    [SerializeField] private Transform agua;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxHeight;
    [SerializeField] private LayerMask laserLayer;

    private bool isBeingHit = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = agua.position;
    }

    private void Update()
    {
        if (isBeingHit)
        {
            if (agua.position.y < initialPosition.y + maxHeight)
            {
                agua.position += Vector3.up * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsLaser(other))
        {
            isBeingHit = true;
        }
    }

    private void LateUpdate()
    {
        isBeingHit = false;
    }

    private bool IsLaser(Collider other)
    {
        return ((1 << other.gameObject.layer) & laserLayer) != 0;
    }
}
