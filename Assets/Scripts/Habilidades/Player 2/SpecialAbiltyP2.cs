using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialAbilityP2 : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform originPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private string freezeTrigger;

    [Header("Freeze Config")]
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private LayerMask affectedLayers;

    [Header("Timing")]
    [SerializeField] private float abilityDuration;
    [SerializeField] private float cooldown;
    [SerializeField] private float animationTime;

    private bool isOnCooldown = false;

    public bool IsActive { get; set; } = false;

    public void FreezeAbility(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isOnCooldown) return;

        StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        isOnCooldown = true;

        if (playerController != null)
        {
            playerController.movementLocked = true;
            print("tirando hab especial");
        }

        if (animator != null)
        {
            animator.SetTrigger(freezeTrigger);
        }

        yield return new WaitForSeconds(animationTime);

        ApplyFreezeArea();

        yield return new WaitForSeconds(abilityDuration);

        if (playerController != null)
        {
            playerController.movementLocked = false;
        }

        yield return new WaitForSeconds(cooldown);

        isOnCooldown = false;
    }

    private void ApplyFreezeArea()
    {
        Vector3 center = originPoint.position + transform.forward * (boxSize.z / 2f);

        Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, transform.rotation, affectedLayers);

        foreach (Collider hit in hits)
        {
            Debug.Log("Congelar a: " + hit.name);

            // Andre, aqui idealmente me ayudarias con los enemigos para congelarlos, ya se que lo hagas por layers o por un script aparte o por tags, como se te haga mas facil
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (originPoint == null) return;

        Gizmos.color = Color.blue;

        Vector3 center = originPoint.position + transform.forward * (boxSize.z / 2f);

        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}