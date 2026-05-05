using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class EarthAbilities : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject rampPrefab;
    [SerializeField] private LayerMask groundMask;
    [Tooltip("Opcional: si no se asigna, buscará automáticamente la MainCamera")]
    [SerializeField] private Transform cameraTransform; // Referencia a la cámara

    [Header("Config General")]
    [SerializeField] private float spawnDistance;

    [Header("Bloque de Tierra")]
    [SerializeField] private float blockCooldownDuration;
    [SerializeField] private float blockLifetime;
    [SerializeField] private float blockRiseHeight;
    [SerializeField] private float appearDuration;
    [SerializeField] private float sinkDistance;
    [SerializeField] private float sinkDuration;
    [SerializeField] private float undergroundOffset;
    [SerializeField] private float floatDuration;

    [Header("Rampa")]
    [SerializeField] private float rampCooldownDuration;
    [SerializeField] private float rampRiseHeight;
    [SerializeField] private float rampAppearDuration;
    [SerializeField] private float rampSinkDistance;
    [SerializeField] private float rampSinkDuration;
    [SerializeField] private float rampUndergroundOffset;

    [Header("Camera Shake")]
    [SerializeField] private float blockRiseShakeIntensity = 0.15f;      // Shake cuando el bloque sube
    [SerializeField] private float blockRiseShakeDuration = 0.3f;
    [SerializeField] private float blockFallShakeIntensity = 0.25f;      // Shake cuando el bloque cae
    [SerializeField] private float blockFallShakeDuration = 0.4f;
    [SerializeField] private float blockSinkShakeIntensity = 0.12f;      // Shake cuando se hunde
    [SerializeField] private float blockSinkShakeDuration = 0.25f;
    [SerializeField] private float rampRiseShakeIntensity = 0.2f;        // Shake cuando la rampa sube
    [SerializeField] private float rampRiseShakeDuration = 0.4f;
    [SerializeField] private float rampSinkShakeIntensity = 0.15f;       // Shake cuando la rampa se hunde
    [SerializeField] private float rampSinkShakeDuration = 0.3f;

    // --- UI ---
    private CooldownUI blockIconUI;
    private CooldownUI rampIconUI;

    private GameObject currentBlock;
    private GameObject currentRamp;

    private bool isBlockOnCooldown = false;
    private bool isRampOnCooldown  = false;
    private bool isBlockCasting    = false;
    private bool isRampCasting     = false;
    private bool isActive          = false;

    private Vector3 originalCameraPosition;
    private bool isShaking = false;

    public bool IsActive
    {
        get => isActive;
        set { isActive = value; enabled = isActive; }
    }

    private Vector3 SpawnOrigin => spawnPoint.position + spawnPoint.forward * spawnDistance;

    private void Start()
    {
        if (AbilityHUD.Instance != null)
        {
            blockIconUI = AbilityHUD.Instance.earthBlockIcon;
            rampIconUI  = AbilityHUD.Instance.earthRampIcon;
        }

        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
                Debug.Log("[EarthAbilities] Cámara principal encontrada automáticamente");
            }
            else
            {
                Debug.LogWarning("[EarthAbilities] No se encontró una MainCamera. El camera shake no funcionará.");
            }
        }

        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }
    }

    // ------------------------- CAMERA SHAKE SYSTEM -------------------------

    private IEnumerator CameraShake(float intensity, float duration)
    {
        if (cameraTransform == null) yield break;

        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            float currentIntensity = intensity * (1f - (elapsed / duration));
            
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * currentIntensity;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * currentIntensity;
            
            cameraTransform.localPosition = originalCameraPosition + new Vector3(offsetX, offsetY, 0f);
            
            yield return null;
        }

        cameraTransform.localPosition = originalCameraPosition;
        isShaking = false;
    }

    // ------------------------- habilidad 1 bloque -------------------------

    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed || isBlockOnCooldown || !playerController.IsGrounded || isBlockCasting)
            return;

        if (currentBlock != null) return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            StartCoroutine(BlockCastSequence(hit.point));
        }
    }

    private IEnumerator BlockCastSequence(Vector3 groundPos)
    {
        isBlockCasting = true;
        playerController.movementLocked = true;
        blockIconUI?.SetUnavailable();

        playerController.GetComponentInChildren<Animator>().SetTrigger("EarthBlock");

        float impactDelay = Mathf.Min(0.2f, appearDuration);
        yield return new WaitForSeconds(impactDelay);

        currentBlock = Instantiate(blockPrefab, groundPos, blockPrefab.transform.rotation);
        StartCoroutine(BlockSequence(currentBlock.transform));

        float remaining = appearDuration - impactDelay;
        if (remaining > 0) yield return new WaitForSeconds(remaining);

        playerController.movementLocked = false;
        isBlockCasting = false;
    }

    private IEnumerator BlockSequence(Transform block)
    {
        Rigidbody rb = block.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        Vector3 finalPos = block.position;
        Vector3 start  = new Vector3(finalPos.x, finalPos.y - undergroundOffset, finalPos.z);
        Vector3 target = new Vector3(finalPos.x, finalPos.y + blockRiseHeight,   finalPos.z);
        block.position = start;

        // SHAKE: Bloque empezando a salir del suelo
        StartCoroutine(CameraShake(blockRiseShakeIntensity, blockRiseShakeDuration));

        float elapsed = 0f;
        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            block.position = Vector3.Lerp(start, target, Mathf.Clamp01(elapsed / appearDuration));
            yield return null;
        }
        block.position = target;

        yield return new WaitForSeconds(floatDuration);

        StartCoroutine(CameraShake(blockFallShakeIntensity, blockFallShakeDuration));

        if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
        yield return new WaitForSeconds(blockLifetime);
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        StartCoroutine(CameraShake(blockSinkShakeIntensity, blockSinkShakeDuration));

        Vector3 sinkStart  = block.position;
        Vector3 sinkTarget = sinkStart - new Vector3(0, sinkDistance, 0);
        elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            block.position = Vector3.Lerp(sinkStart, sinkTarget, Mathf.Clamp01(elapsed / sinkDuration));
            yield return null;
        }

        if (block != null) { Destroy(block.gameObject); currentBlock = null; }

        blockIconUI?.SetOnCooldown(blockCooldownDuration);
        StartCoroutine(BlockCooldownRoutine());
    }

    private IEnumerator BlockCooldownRoutine()
    {
        isBlockOnCooldown = true;
        yield return new WaitForSeconds(blockCooldownDuration);
        isBlockOnCooldown = false;
    }

    // ------------------------- habilidad 2 rampa -------------------------

    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed || isRampOnCooldown || !playerController.IsGrounded || isRampCasting)
            return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            StartCoroutine(RampCastSequence(hit.point));
        }
    }

    private IEnumerator RampCastSequence(Vector3 groundPos)
    {
        isRampCasting = true;
        playerController.movementLocked = true;
        rampIconUI?.SetUnavailable();

        if (currentRamp != null)
        {
            StartCoroutine(SinkAndDestroy(currentRamp.transform));
            currentRamp = null;
        }

        playerController.GetComponentInChildren<Animator>().SetTrigger("EarthRamp");

        float impactDelay = Mathf.Min(0.2f, rampAppearDuration);
        yield return new WaitForSeconds(impactDelay);

        Quaternion finalRot = Quaternion.LookRotation(spawnPoint.forward, Vector3.up) * rampPrefab.transform.rotation;
        currentRamp = Instantiate(rampPrefab, groundPos - new Vector3(0, rampUndergroundOffset, 0), finalRot);
        
        StartCoroutine(CameraShake(rampRiseShakeIntensity, rampRiseShakeDuration));
        
        StartCoroutine(RaiseFromGround(currentRamp.transform, rampRiseHeight + rampUndergroundOffset, rampAppearDuration));

        float remaining = rampAppearDuration - impactDelay;
        if (remaining > 0) yield return new WaitForSeconds(remaining);

        playerController.movementLocked = false;
        isRampCasting = false;

        rampIconUI?.SetOnCooldown(rampCooldownDuration);
        StartCoroutine(RampCooldownRoutine());
    }

    private IEnumerator RampCooldownRoutine()
    {
        isRampOnCooldown = true;
        yield return new WaitForSeconds(rampCooldownDuration);
        isRampOnCooldown = false;
    }

    private IEnumerator SinkAndDestroy(Transform ramp)
    {
        StartCoroutine(CameraShake(rampSinkShakeIntensity, rampSinkShakeDuration));

        Vector3 sinkStart  = ramp.position;
        Vector3 sinkTarget = sinkStart - new Vector3(0, rampSinkDistance, 0);

        float elapsed = 0f;
        while (elapsed < rampSinkDuration)
        {
            if (ramp == null) yield break;
            elapsed += Time.deltaTime;
            ramp.position = Vector3.Lerp(sinkStart, sinkTarget, Mathf.Clamp01(elapsed / rampSinkDuration));
            yield return null;
        }

        if (ramp != null) Destroy(ramp.gameObject);
    }

    // ------------------------- utilidades -------------------------

    private IEnumerator RaiseFromGround(Transform ramp, float riseHeight, float duration)
    {
        Vector3 startPos  = ramp.position;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + riseHeight, startPos.z);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ramp.position = Vector3.Lerp(startPos, targetPos, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        ramp.position = targetPos;
    }

    public bool IsUsingAbility()
    {
        return isBlockOnCooldown || isRampOnCooldown || isBlockCasting || isRampCasting;
    }
}