using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class SpecialAbility : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject specialBlockPrefab;
    [SerializeField] private LayerMask groundMask;

    [Header("Config")]
    [SerializeField] private float spawnDistance;
    [SerializeField] private float cooldownDuration;
    [SerializeField] float castTime;

    [Header("Efecto")]
    [SerializeField] private float riseHeight;
    [SerializeField] private float riseDuration;
    [SerializeField] private float undergroundOffset;

    private bool isOnCooldown = false;
    private bool isCasting = false;

    private Vector3 SpawnOrigin => spawnPoint.position + spawnPoint.forward * spawnDistance;

    public void HabilidadEspecial(InputAction.CallbackContext context)
    {
        if (!context.performed || isOnCooldown || isCasting || !playerController.IsGrounded)
            return;

        if (Physics.Raycast(SpawnOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            StartCoroutine(CastSequence(() =>
            {
                GameObject block = Instantiate(specialBlockPrefab, hit.point, Quaternion.identity);
                StartCoroutine(RaiseFromGround(block.transform));
                print("hab especial");
            }));
        }
    }

    private IEnumerator CastSequence(Action spawnAction)
    {
        isCasting = true;
        playerController.movementLocked = true;

        playerController.GetComponentInChildren<Animator>().SetTrigger("Special");

        spawnAction.Invoke();

        yield return new WaitForSeconds(castTime);

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

    private IEnumerator RaiseFromGround(Transform obj)
    {
        Vector3 finalPos = obj.position;

        Vector3 start = new Vector3(
            finalPos.x,
            finalPos.y - undergroundOffset,
            finalPos.z
        );

        Vector3 target = new Vector3(
            finalPos.x,
            finalPos.y + riseHeight,
            finalPos.z
        );

        obj.position = start;

        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / riseDuration);
            obj.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        obj.position = target;
    }
}