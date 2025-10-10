using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class AbilityProjectile : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float launchHeight;   
    [SerializeField] private float launchDuration; 

    private Rigidbody rb;
    private Action onComplete;
    private bool isLaunched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true; 
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

    public void Launch(Vector3 start, Vector3 target, Action onCompleteCallback)
    {
        transform.position = start;
        rb.isKinematic = false; 
        rb.linearVelocity = CalculateLaunchVelocity(start, target, launchHeight, launchDuration);
        onComplete = onCompleteCallback;
        isLaunched = true;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 target, float height, float duration)
    {
        Vector3 horizontal = target - start;
        horizontal.y = 0f;
        Vector3 horizontalVel = horizontal / duration;

        float displacementY = target.y - start.y;

        float verticalVel = (2 * height / duration) + (displacementY / duration);

        verticalVel = (displacementY + 0.5f * Mathf.Abs(Physics.gravity.y) * duration * duration) / duration;

        return horizontalVel + Vector3.up * verticalVel;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!isLaunched)
        {
            return;
        } 
        StopProjectile();
    }

    public void StopProjectile()
    {
        isLaunched = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true; 
        onComplete?.Invoke();
        onComplete = null;
    }

    public void ResetToSpawn(Transform spawn)
    {
        StopProjectile();
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
    }
}
