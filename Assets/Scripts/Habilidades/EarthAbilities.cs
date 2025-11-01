using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EarthAbilities : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject rampPrefab;
    [SerializeField] private LayerMask groundMask;

    [Header("Configuración General")]
    [SerializeField] private float spawnDistance;
    [SerializeField] private float cooldownDuration;

    [Header("Bloque de Tierra")]
    [SerializeField] private float blockLifetime;
    [SerializeField] private float blockRiseHeight;
    [SerializeField] private float appearDuration;
    [SerializeField] private float sinkDistance;
    [SerializeField] private float sinkDuration;
    [SerializeField] private float undergroundOffset;

    [Header("Rampa")]
    [SerializeField] private float rampRiseHeight;
    [SerializeField] private float rampAppearDuration;

    private GameObject currentBlock;
    private GameObject currentRamp;
    private bool isOnCooldown = false;
    private bool isActive = false;

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

    // BLOQUE
    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed || isOnCooldown)
            return;

        if (currentBlock != null)
            return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 groundPos = hit.point;
            Quaternion rotation = blockPrefab.transform.rotation;
            currentBlock = Instantiate(blockPrefab, groundPos, rotation);

            StartCoroutine(RaiseFromGround(currentBlock.transform, blockRiseHeight, appearDuration));
            StartCoroutine(BlockLifetimeRoutine());
            StartCoroutine(CooldownRoutine());
        }
        else
        {
            print("no hay suelo xd");
        }
    }

    private IEnumerator BlockLifetimeRoutine()
    {
        yield return new WaitForSeconds(blockLifetime);

        if (currentBlock != null)
        {
            Vector3 start = currentBlock.transform.position;
            Vector3 target = start - new Vector3(0, sinkDistance, 0);

            float elapsed = 0f;
            while (elapsed < sinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / sinkDuration);
                currentBlock.transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }

            Destroy(currentBlock);
            currentBlock = null;
        }
    }


    // RAMPA
    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed || isOnCooldown)
            return;

        if (currentRamp != null)
            return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 groundPos = hit.point;

            Quaternion playerRot = Quaternion.LookRotation(spawnPoint.forward, Vector3.up);
            Quaternion prefabRot = rampPrefab.transform.rotation;
            Quaternion finalRot = playerRot * prefabRot; 

            currentRamp = Instantiate(rampPrefab, groundPos, finalRot);

            StartCoroutine(RaiseFromGround(currentRamp.transform, rampRiseHeight, rampAppearDuration));
            StartCoroutine(CooldownRoutine());
        }
        else
        {
            print("no hay suelo xd");
        }
    }


    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }

    private IEnumerator RaiseFromGround(Transform obj, float riseHeight, float duration)
    {
        Vector3 finalPos = obj.position;
        Vector3 start = new Vector3(finalPos.x, finalPos.y - undergroundOffset, finalPos.z);
        Vector3 target = new Vector3(finalPos.x, finalPos.y + riseHeight, finalPos.z);

        obj.position = start;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        obj.position = target;
    }


    public bool IsUsingAbility()
    {
        return isOnCooldown;
    }
}
