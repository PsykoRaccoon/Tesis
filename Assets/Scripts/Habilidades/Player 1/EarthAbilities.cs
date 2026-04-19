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

    [Header("Config General")]
    [SerializeField] private float spawnDistance;
    [SerializeField] private float cooldownDuration;

    [Header("Bloque de Tierra")]
    [SerializeField] private float blockLifetime;
    [SerializeField] private float blockRiseHeight;
    [SerializeField] private float appearDuration; 
    [SerializeField] private float sinkDistance;
    [SerializeField] private float sinkDuration;
    [SerializeField] private float undergroundOffset;
    [SerializeField] private float floatDuration;

    [Header("Rampa")]
    [SerializeField] private float rampRiseHeight;
    [SerializeField] private float rampAppearDuration;

    private GameObject currentBlock;
    private GameObject currentRamp;
    private bool isOnCooldown = false;
    private bool isActive = false;
    private bool isCasting = false;

    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            enabled = isActive;
        }
    }

    private Vector3 SpawnOrigin => spawnPoint.position + spawnPoint.forward * spawnDistance;

    // ------------------------- habilidad 1 bloque -------------------------
    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed || isOnCooldown || !playerController.IsGrounded || isCasting)
            return;

        if (currentBlock != null) return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            StartCoroutine(CastSequence(() =>
            {
                Vector3 groundPos = hit.point;
                Quaternion rotation = blockPrefab.transform.rotation;
                currentBlock = Instantiate(blockPrefab, groundPos, rotation);

                StartCoroutine(BlockSequence(currentBlock.transform));
            }, "EarthBlock", appearDuration));
        }
    }

    // ------------------------- habilidad 2 rampa -------------------------
    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed || isOnCooldown || !playerController.IsGrounded || isCasting)
            return;

        if (currentRamp != null) return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            StartCoroutine(CastSequence(() =>
            {
                Vector3 groundPos = hit.point;
                Quaternion playerRot = Quaternion.LookRotation(spawnPoint.forward, Vector3.up);
                Quaternion prefabRot = rampPrefab.transform.rotation;
                Quaternion finalRot = playerRot * prefabRot;

                currentRamp = Instantiate(rampPrefab, groundPos, finalRot);

                StartCoroutine(RaiseFromGround(currentRamp.transform, rampRiseHeight, rampAppearDuration));
            }, "EarthRamp", rampAppearDuration));
        }
    }

    // ------------------------- cast -------------------------

    private IEnumerator CastSequence(Action spawnAction, string triggerName, float duration)
    {
        isCasting = true;
        playerController.movementLocked = true;

        playerController.GetComponentInChildren<Animator>().SetTrigger(triggerName);

        float impactDelay = 0.2f;

        if (duration < impactDelay) impactDelay = duration;

        yield return new WaitForSeconds(impactDelay);

        spawnAction.Invoke();

        float remainingTime = duration - impactDelay;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        playerController.movementLocked = false;
        isCasting = false;
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }

    private IEnumerator BlockSequence(Transform block)
    {
        Rigidbody rb = block.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Vector3 finalPos = block.position;
        Vector3 start = new Vector3(finalPos.x, finalPos.y - undergroundOffset, finalPos.z);
        Vector3 target = new Vector3(finalPos.x, finalPos.y + blockRiseHeight, finalPos.z);

        block.position = start;

        float elapsed = 0f;

        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / appearDuration);
            block.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        block.position = target;

        yield return new WaitForSeconds(floatDuration);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        yield return new WaitForSeconds(blockLifetime);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Vector3 sinkStart = block.position;
        Vector3 sinkTarget = sinkStart - new Vector3(0, sinkDistance, 0);

        elapsed = 0f;

        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / sinkDuration);
            block.position = Vector3.Lerp(sinkStart, sinkTarget, t);
            yield return null;
        }

        if (block != null)
        {
            Destroy(block.gameObject);
            currentBlock = null;
        }
    }

    private IEnumerator RaiseFromGround(Transform ramp, float riseHeight, float duration)
    {
        Vector3 startPos = ramp.position;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + riseHeight, startPos.z);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            ramp.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        ramp.position = targetPos;
    }

    public bool IsUsingAbility()
    {
        return isOnCooldown || isCasting;
    }
}