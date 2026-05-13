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
    [SerializeField] private float upwardModifier;
    [SerializeField] private LayerMask affectedLayers;

    [Header("Timing")]
    [SerializeField] private float abilityDuration;
    [SerializeField] private float cooldown;
    [SerializeField] private float animationTime;

    [Header("Particles")]
    [SerializeField] private GameObject repulsionParticles;
    [SerializeField] private GameObject attractionParticles;

    // --- UI ---
    private CooldownUI repulsionIconUI;
    private CooldownUI attractionIconUI;

    private bool isRepulsionOnCooldown = false;
    private bool isAttractionOnCooldown = false;

    public bool IsActive { get; set; } = false;

    private void Start()
    {
        if (AbilityHUD.Instance != null)
        {
            repulsionIconUI  = AbilityHUD.Instance.airRepulsionIcon;
            attractionIconUI = AbilityHUD.Instance.airAttractionIcon;
        }
    }

    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!IsActive || !context.performed || isRepulsionOnCooldown) return;
        StartCoroutine(ForceWaveRoutine());
    }

    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!IsActive || !context.performed || isAttractionOnCooldown) return;
        StartCoroutine(PullRoutine());
    }

    //////////////////////////////////// Logica de habilidad de empuje ////////////////////////////////////

    private IEnumerator ForceWaveRoutine()
    {
        isRepulsionOnCooldown = true;
        repulsionIconUI?.SetUnavailable();

        if (playerController != null) playerController.movementLocked = true;
        if (animator != null) animator.SetTrigger(repulsionTrigger);

        yield return new WaitForSeconds(animationTime);
        if (repulsionParticles != null) repulsionParticles.SetActive(true);

        ApplyForceWave();

        yield return new WaitForSeconds(abilityDuration);

        if (repulsionParticles != null) repulsionParticles.SetActive(false);

        if (playerController != null) playerController.movementLocked = false;

        repulsionIconUI?.SetOnCooldown(cooldown);

        yield return new WaitForSeconds(cooldown);

        isRepulsionOnCooldown = false;
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
                rb.AddForce(transform.forward * force, ForceMode.Impulse);
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
        isAttractionOnCooldown = true;
        attractionIconUI?.SetUnavailable();

        if (playerController != null) playerController.movementLocked = true;
        if (animator != null) animator.SetTrigger(attractionTrigger);

        yield return new WaitForSeconds(animationTime);

        if (attractionParticles != null) attractionParticles.SetActive(true);

        ApplyPullWave();

        yield return new WaitForSeconds(abilityDuration);

        if (attractionParticles != null) attractionParticles.SetActive(false);

        if (playerController != null) playerController.movementLocked = false;

        attractionIconUI?.SetOnCooldown(cooldown);

        yield return new WaitForSeconds(cooldown);

        isAttractionOnCooldown = false;
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