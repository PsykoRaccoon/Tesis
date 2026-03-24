using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirAbilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform originPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTriggerName = "Repulsion";

    [Header("Repulsion Config")]
    [SerializeField] private Vector3 boxSize = new Vector3();
    [SerializeField] private float force;
    [SerializeField] private float upwardModifier ;
    [SerializeField] private LayerMask affectedLayers;

    [Header("Timing")]
    [SerializeField] private float abilityDuration; //tiempo bloqueado despues de haber activado la hab
    [SerializeField] private float cooldown; //cooldown xd
    [SerializeField] private float animationTime; //delay antes de que se active la hab
    private bool isOnCooldown = false;

    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isOnCooldown) return;

        StartCoroutine(ForceWaveRoutine());
    }

    private IEnumerator ForceWaveRoutine()
    {
        isOnCooldown = true;

        if (playerController != null)
            playerController.movementLocked = true;

        if (animator != null)
        {
            animator.SetTrigger(animationTriggerName);
        }

        yield return new WaitForSeconds(animationTime); 

        ApplyForceWave();

        yield return new WaitForSeconds(abilityDuration);

        if (playerController != null)
            playerController.movementLocked = false;

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
}
