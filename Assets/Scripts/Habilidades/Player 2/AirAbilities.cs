using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirAbilities : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform originPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private string repulsionTrigger;
    [SerializeField] private string attractionTrigger;


    [Header("Repulsion Config")]
    [SerializeField] private Vector3 boxSize = new Vector3();
    [SerializeField] private float force;
    [SerializeField] private float upwardModifier ;
    [SerializeField] private LayerMask affectedLayers;

    [Header("Timing")]
    [SerializeField] private float abilityDuration; //tiempo que se mantiene bloqueado despues de lanzar la hab
    [SerializeField] private float cooldown; //cooldown xd 
    [SerializeField] private float animationTime; //tiempo antes de que salga la hab
    private bool isOnCooldown = false;

    public bool IsActive { get; set; } = false;

    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!IsActive) return;
        if (!context.performed) return;
        if (isOnCooldown) return;

        StartCoroutine(ForceWaveRoutine());
    }

    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!IsActive) return;
        if (!context.performed) return;
        if (isOnCooldown) return;

        StartCoroutine(PullRoutine());
    }

    //////////////////////////////////// Logica de habilidad de empuje ////////////////////////////////////
    private IEnumerator ForceWaveRoutine()
    {
        isOnCooldown = true;

        if (playerController != null)
        {
            playerController.movementLocked = true;
        }

        if (animator != null)
        {
            animator.SetTrigger(repulsionTrigger);
        }

        yield return new WaitForSeconds(animationTime); 

        ApplyForceWave();

        yield return new WaitForSeconds(abilityDuration);

        if (playerController != null)
        {
            playerController.movementLocked = false;
        }

        yield return new WaitForSeconds(cooldown);

        isOnCooldown = false;
    }

    private void ApplyForceWave()
    {
        Vector3 center = originPoint.position + transform.forward * (boxSize.z / 2f);

        Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, transform.rotation, affectedLayers);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;

            if (rb != null)
            {
                Vector3 direction = transform.forward;

                rb.AddForce(direction * force, ForceMode.Impulse);
                rb.AddForce(Vector3.up * upwardModifier, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (originPoint == null) return;

        Gizmos.color = Color.cyan;

        Vector3 center = originPoint.position + transform.forward * (boxSize.z / 2f);

        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    //////////////////////////////////// Logica de habilidad de atraccion ////////////////////////////////////
    private IEnumerator PullRoutine()
    {
        isOnCooldown = true;

        if (playerController != null)
        {
            playerController.movementLocked = true;
        }

        if (animator != null)
        {
            animator.SetTrigger(attractionTrigger);
            
        }

        yield return new WaitForSeconds(animationTime);

        ApplyPullWave();

        yield return new WaitForSeconds(abilityDuration);

        if (playerController != null)
        {
            playerController.movementLocked = false;
            
        }

        yield return new WaitForSeconds(cooldown);

        isOnCooldown = false;
    }

    private void ApplyPullWave()
    {
        Vector3 center = originPoint.position + transform.forward * (boxSize.z / 2f);

        Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, transform.rotation, affectedLayers);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;

            if (rb != null && !rb.isKinematic)
            {
                Vector3 direction = (originPoint.position - hit.transform.position).normalized;

                rb.AddForce(direction * force, ForceMode.Impulse);
                rb.AddForce(Vector3.up * upwardModifier, ForceMode.Impulse);
            }
        }
    }
}
