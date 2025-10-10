using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AbilityController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject groundMarkerPrefab;

    [Header("Configuración")]
    [SerializeField] private float aimDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float cooldownDuration;

    private AbilityProjectile currentProjectile;
    private GameObject currentMarker;

    private bool isAiming = false;
    private bool isOnCooldown = false;
    private Vector3 targetPosition;

    private void Start()
    {
        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        currentProjectile = proj.GetComponent<AbilityProjectile>();
        proj.SetActive(false);

        currentMarker = Instantiate(groundMarkerPrefab, spawnPoint.position, Quaternion.identity);
        currentMarker.SetActive(false);
    }

    private void Update()
    {
        if (isAiming)
        {
            UpdateMarkerPosition();
            currentProjectile.FollowSpawn(spawnPoint);
        }
    }

    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!context.performed || isOnCooldown)
        {
            return;
        }

        if (!isAiming)
        {
            StartAiming();
        }
        else
        {
            LaunchProjectile();
        }
    }

    private void StartAiming()
    {
        isAiming = true;
        currentProjectile.gameObject.SetActive(true);
        currentMarker.SetActive(true);
    }

    private void LaunchProjectile()
    {
        isAiming = false;
        currentMarker.SetActive(false);
        currentProjectile.Launch(spawnPoint.position, targetPosition, OnProjectileComplete);
    }

    private void UpdateMarkerPosition()
    {
        Vector3 rayOrigin = spawnPoint.position + spawnPoint.forward * aimDistance + Vector3.up * 2f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 10f, groundMask))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = spawnPoint.position + spawnPoint.forward * aimDistance;
        }
        currentMarker.transform.position = targetPosition;
    }

    private void OnProjectileComplete()
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);

        //currentProjectile.ResetToSpawn(spawnPoint); tal vez sea esto, idk, asi funciona bien xd
        currentProjectile.gameObject.SetActive(false);

        isOnCooldown = false;
    }

}
