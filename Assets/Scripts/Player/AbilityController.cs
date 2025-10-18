using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AbilityController : MonoBehaviour
{
    [Header("Referencias bola de fuego")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject groundMarkerPrefab;

    [Header("Configuracion")]
    [SerializeField] private float aimDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float cooldownDuration;

    private AbilityProjectile currentProjectile;
    private GameObject currentMarker;

    private bool projectileLaunched = false;
    private bool isAiming = false;
    private bool isOnCooldown = false;
    private Vector3 targetPosition;

    //-------------------------------------------------------------------------------------\\

    [Header("Referencias rayo laser")] 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform aimPivot; 
    [SerializeField] private GameObject laserBeamPrefab;

    [Header("Configuracion del Laser")]
    [SerializeField] private float verticalAimSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0.05f, 1f)] private float rotationSmoothTime;

    private GameObject currentLaser;
    private bool isUsingLaser = false;
    private Vector2 aimInput;

    private bool IsAnyAbilityActive => isAiming || isUsingLaser;

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
        // --- bola de fuego --- \\
        if (isAiming)
        {
            UpdateMarkerPosition();
            currentProjectile.FollowSpawn(spawnPoint);
        }

        // --- laser --- \\
        if (isUsingLaser)
        {
            HandleLaserRotation();
        }
    }


    // ---------------------------------------------------------------------------- logica bola de fuego ---------------------------------------------------------------------- \\

    public void Habilidad1(InputAction.CallbackContext context) //bola de fuego
    {
        if (!context.performed || isOnCooldown || isUsingLaser || projectileLaunched)
            return;

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
        projectileLaunched = true;
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
        projectileLaunched = false;
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

    // ------------------------------------------------------------------------------- logica rayo laser ----------------------------------------------------------------------------------- \\
    public void Habilidad2(InputAction.CallbackContext context) //rayo laser
    {
        if (context.performed && !isAiming)
        {
            StartLaser();
        }
        else if (context.canceled && isUsingLaser)
        {
            StopLaser();
        }
    }

    private void StartLaser()
    {
        aimPivot.localRotation = Quaternion.Euler(0, 0, 0);
        if (isUsingLaser) 
        {
            return;  
        } 

        isUsingLaser = true;

        playerController.movementLocked = true;

        if (currentLaser == null)
        {
            currentLaser = Instantiate(laserBeamPrefab, spawnPoint.position, spawnPoint.rotation);

        }
        else
        {
            currentLaser.SetActive(true);
        }
    }

    private void StopLaser()
    {
        if (!isUsingLaser)
        {
            return;
        }

        isUsingLaser = false;

        playerController.movementLocked = false;

        if (currentLaser != null)
        {
            currentLaser.SetActive(false);
        }
    }

    public void AimVertical(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
    }

    private void HandleLaserRotation()
    {
        Vector2 moveInput = playerController.GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (inputDir.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / rotationSmoothTime);
        }

        float vertical = aimInput.y;
        float currentX = aimPivot.localEulerAngles.x;
        if (currentX > 180)
        {
            currentX -= 360;
        }
        currentX = Mathf.Clamp(currentX - vertical * verticalAimSpeed * Time.deltaTime, -45f, 45f);
        aimPivot.localRotation = Quaternion.Euler(currentX, 0, 0);

        if (currentLaser != null)
        {
            currentLaser.transform.position = spawnPoint.position;
            currentLaser.transform.rotation = aimPivot.rotation;
        }
    }
}
