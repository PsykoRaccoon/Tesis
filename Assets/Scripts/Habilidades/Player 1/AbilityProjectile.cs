using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class AbilityProjectile : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float launchHeight;
    [SerializeField] private float launchDuration;

    [Header("Lifetime")]
    [SerializeField] private float maxLifetime;

    private Rigidbody rb;
    private Action onComplete;
    private bool isLaunched = false;

    private float lifeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (!isLaunched) return;

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= maxLifetime)
        {
            StopProjectile();
        }
    }

    public void FollowSpawn(Transform spawn)
    {
        if (isLaunched)
        {
            return;
        }
        rb.isKinematic = true;
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
    }

    public Vector3 GetLaunchVelocityPreview(Vector3 start, Vector3 target)
    {
        return CalculateLaunchVelocity(start, target, launchHeight, launchDuration);
    }

    public void Launch(Vector3 start, Vector3 target, Action onCompleteCallback)
    {
        transform.position = start;
        rb.isKinematic = false;
        rb.linearVelocity = CalculateLaunchVelocity(start, target, launchHeight, launchDuration);

        onComplete = onCompleteCallback;
        isLaunched = true;

        lifeTimer = 0f;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 target, float height, float duration)
    {
        Vector3 horizontal = target - start;
        horizontal.y = 0f;
        Vector3 horizontalVel = horizontal / duration;

        float displacementY = target.y - start.y;
        float verticalVel = (displacementY + 0.5f * Mathf.Abs(Physics.gravity.y) * duration * duration) / duration;

        return horizontalVel + Vector3.up * verticalVel;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bola tocó: " + other.name + " | Tag: " + other.tag);

        if (!isLaunched)
        {
            return;
        }
        StopProjectile();
    }

    private void FixedUpdate()
{
    if (!isLaunched) return;

    Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);

    foreach (var hit in hits)
    {
        if (hit.CompareTag("Player"))
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(1, DamageType.Player); 
                Debug.Log("Daño aplicado al player");
            }

            StopProjectile();
            break;
        }
    }
}

    public void StopProjectile()
    {
        isLaunched = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        onComplete?.Invoke();
        onComplete = null;

        Destroy(gameObject);
    }
}