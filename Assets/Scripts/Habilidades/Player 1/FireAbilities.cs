using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FireAbilities : MonoBehaviour
{
    public bool IsActive { get; set; } = true;

    [Header("Referencias bola de fuego")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject groundMarkerPrefab;
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int resolution;
    [SerializeField] private float timeStep;
    [SerializeField] private LayerMask trajectoryCollisionMask;

    [Header("Config")]
    [SerializeField] private float aimDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float cooldownDuration;

    [Header("Animaciones")]
    [SerializeField] private string aimBoolName;
    [SerializeField] private string throwTriggerName;
    [SerializeField] private Animator animator;
    [SerializeField] private string laserBoolName;

    [Header("Laser Timing")]
    [SerializeField] private float laserStartDelay;

    // --- UI Cooldown ---
    private CooldownUI fireballIconUI;
    private CooldownUI laserIconUI;

    private AbilityProjectile currentProjectile;
    private GameObject currentMarker;

    private bool projectileLaunched = false;
    private bool isAiming = false;
    private bool isOnCooldown = false;
    private Vector3 targetPosition;

    //-------------------------------------------------------------------------------------\\

    [Header("Laser Refs")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private GameObject laserBeamPrefab;
    [SerializeField] private Transform laserSpawnPoint;

    [Header("Laser Config")]
    [SerializeField] private float verticalAimSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0.05f, 1f)] private float rotationSmoothTime;

    private GameObject currentLaser;
    private bool isUsingLaser = false;
    private Vector2 aimInput;

    private bool IsAnyAbilityActive => isAiming || isUsingLaser;

    public bool IsUsingAbility()
    {
        return isAiming || isUsingLaser || isOnCooldown;
    }

    private void Start()
    {
        currentMarker = Instantiate(groundMarkerPrefab, spawnPoint.position, Quaternion.identity);
        currentMarker.SetActive(false);

        if (AbilityHUD.Instance != null)
        {
            fireballIconUI = AbilityHUD.Instance.fireballIcon;
            laserIconUI    = AbilityHUD.Instance.laserIcon;
        }
    }

    private void Update()
    {
        if (!IsActive) return;

        if (!playerController.IsGrounded)
        {
            if (isAiming) CancelAiming();
            if (isUsingLaser) StopLaser();
            return;
        }

        // --- bola de fuego --- \\
        if (isAiming && currentProjectile != null)
        {
            UpdateMarkerPosition();
            DrawTrajectory();
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
        if (!IsActive || !playerController.IsGrounded) return;

        if (!context.performed || isOnCooldown || isUsingLaser || projectileLaunched)
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
        if (!playerController.IsGrounded) return;

        isAiming = true;

        fireballIconUI?.SetUnavailable();

        animator.SetBool(aimBoolName, true);

        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        currentProjectile = proj.GetComponent<AbilityProjectile>();

        currentMarker.SetActive(true);
        trajectoryLine.enabled = true;
    }

    private void CancelAiming()
    {
        isAiming = false;
        animator.SetBool(aimBoolName, false);
        currentMarker.SetActive(false);
        trajectoryLine.enabled = false;

        if (currentProjectile != null)
        {
            Destroy(currentProjectile.gameObject);
            currentProjectile = null;
        }

        if (!isOnCooldown)
            fireballIconUI?.SetAvailable();
    }

    private void LaunchProjectile()
    {
        if (currentProjectile == null) return;

        isAiming = false;

        animator.SetBool(aimBoolName, false);
        animator.SetTrigger(throwTriggerName); 

        projectileLaunched = true;
        currentMarker.SetActive(false);
        trajectoryLine.enabled = false;

        fireballIconUI?.SetUnavailable();

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
        currentProjectile = null;

        fireballIconUI?.SetOnCooldown(cooldownDuration);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }

    private void DrawTrajectory()
    {
        if (currentProjectile == null) return;

        Vector3 start = spawnPoint.position;
        Vector3 velocity = currentProjectile.GetLaunchVelocityPreview(start, targetPosition);

        trajectoryLine.positionCount = resolution;

        Vector3 previousPoint = start;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;

            Vector3 currentPoint = start
                + velocity * t
                + 0.5f * Physics.gravity * t * t;

            Vector3 direction = currentPoint - previousPoint;
            float distance = direction.magnitude;

            if (Physics.Raycast(previousPoint, direction.normalized, out RaycastHit hit, distance, trajectoryCollisionMask))
            {
                trajectoryLine.positionCount = i + 1;
                trajectoryLine.SetPosition(i, hit.point);
                break;
            }

            trajectoryLine.SetPosition(i, currentPoint);
            previousPoint = currentPoint;
        }
    }

    // ------------------------------------------------------------------------------- logica rayo laser ----------------------------------------------------------------------------------- \\

    public void Habilidad2(InputAction.CallbackContext context) //rayo laser
    {
        if (!IsActive) return;

        if (context.performed && !isAiming && playerController.IsGrounded)
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
        if (isUsingLaser) return;

        isUsingLaser = true;
        playerController.movementLocked = true;

        laserIconUI?.SetUnavailable();

        if (animator != null)
        {
            animator.SetBool(laserBoolName, true);
        }

        StartCoroutine(LaserDelayRoutine());
    }

    private IEnumerator LaserDelayRoutine()
    {
        yield return new WaitForSeconds(laserStartDelay);
        if (!isUsingLaser) yield break;

        if (currentLaser == null)
        {
            currentLaser = Instantiate(laserBeamPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
            currentLaser.transform.SetParent(laserSpawnPoint);
        }
        else
        {
            currentLaser.SetActive(true);
        }
    }

    private void StopLaser()
    {
        if (!isUsingLaser) return;

        isUsingLaser = false;
        playerController.movementLocked = false;
        animator.SetBool(laserBoolName, false);

        if (currentLaser != null)
        {
            currentLaser.SetActive(false);
        }

        laserIconUI?.SetAvailable();
    }

    public void AimVertical(InputAction.CallbackContext context)
    {
        if (!IsActive) return;
        aimInput = context.ReadValue<Vector2>();
    }

    private void HandleLaserRotation()
    {
        Vector2 moveInput = playerController.GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(moveInput.y, 0, -moveInput.x).normalized;

        if (inputDir.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / rotationSmoothTime);
        }

        float vertical = aimInput.y;
        float currentX = aimPivot.localEulerAngles.x;
        if (currentX > 180) currentX -= 360;

        currentX = Mathf.Clamp(currentX - vertical * verticalAimSpeed * Time.deltaTime, -45f, 45f);
        aimPivot.localRotation = Quaternion.Euler(currentX, 0, 0);
    }
}