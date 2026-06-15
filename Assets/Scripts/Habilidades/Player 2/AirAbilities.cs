using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirAbilities : MonoBehaviour, IAbility
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

    [Header("VFX")]
    [SerializeField] private GameObject vfxObject;
    [SerializeField] private Transform repulsionSpawnPoint;
    [SerializeField] private Transform attractionSpawnPoint;
    [SerializeField] private float vfxTravelDistance;
    [SerializeField] private float vfxTravelDuration;
    [SerializeField] private float vfxShrinkDuration;

    // --- UI ---
    private CooldownUI repulsionIconUI;
    private CooldownUI attractionIconUI;

    private bool isRepulsionOnCooldown = false;
    private bool isAttractionOnCooldown = false;
    public bool IsUsingAbility() => isRepulsionOnCooldown || isAttractionOnCooldown;
    private bool isActive = false;
    public bool IsActive
    {
        get => isActive;
        set { isActive = value; enabled = isActive; }
    }

    private void Awake()
    {
        if (AbilityHUD.Instance != null)
        {
            repulsionIconUI  = AbilityHUD.Instance.airRepulsionIcon;
            attractionIconUI = AbilityHUD.Instance.airAttractionIcon;
        }
    }

    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!enabled || !IsActive || !context.performed || isRepulsionOnCooldown) return;
        StartCoroutine(ForceWaveRoutine());
    }

    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!enabled || !IsActive || !context.performed || isAttractionOnCooldown) return;
        StartCoroutine(PullRoutine());
    }

    //////////////////////////////////// VFX ////////////////////////////////////

    private IEnumerator MoveVFX(Vector3 startPos, Vector3 endPos, Quaternion rotation)
    {
        vfxObject.transform.position = startPos;
        vfxObject.transform.rotation = rotation;
        vfxObject.transform.localScale = Vector3.one;
        vfxObject.SetActive(true);

        // Fase 1: viaje
        float elapsed = 0f;
        while (elapsed < vfxTravelDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / vfxTravelDuration;
            vfxObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Fase 2: shrink
        elapsed = 0f;
        while (elapsed < vfxShrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / vfxShrinkDuration;
            vfxObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        vfxObject.transform.localScale = Vector3.one;
        vfxObject.SetActive(false);
    }

    //////////////////////////////////// Logica de habilidad de empuje ////////////////////////////////////

    private IEnumerator ForceWaveRoutine()
    {
        isRepulsionOnCooldown = true;
        repulsionIconUI?.SetUnavailable();

        if (playerController != null) playerController.movementLocked = true;
        if (animator != null) animator.SetTrigger(repulsionTrigger);

        yield return new WaitForSeconds(animationTime);

        Vector3 startPos = repulsionSpawnPoint.position;
        Vector3 endPos   = startPos + transform.forward * vfxTravelDistance;
        StartCoroutine(MoveVFX(startPos, endPos, repulsionSpawnPoint.rotation));

        ApplyForceWave();

        yield return new WaitForSeconds(abilityDuration);

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

        Vector3 startPos = attractionSpawnPoint.position;
        Vector3 endPos   = repulsionSpawnPoint.position;
        Quaternion flippedRotation = attractionSpawnPoint.rotation * Quaternion.Euler(0f, 180f, 0f);
        StartCoroutine(MoveVFX(startPos, endPos, flippedRotation));

        ApplyPullWave();

        yield return new WaitForSeconds(abilityDuration);

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

    public void LockVisuals()
    {
        repulsionIconUI?.SetLocked();
        attractionIconUI?.SetLocked();
    }

    public void UnlockVisuals()
    {
        repulsionIconUI?.SetUnlocked();
        attractionIconUI?.SetUnlocked();
    }
}