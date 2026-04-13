using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class WaterAbilities : MonoBehaviour
{
    public bool IsActive { get; set; } = true;

    [Header("Laser Refs")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private GameObject laserBeamPrefab;

    [Header("Laser Config")]
    [SerializeField] private float verticalAimSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0.05f, 1f)] private float rotationSmoothTime;
    [SerializeField] private float delay;
    [SerializeField] private Animator animator;

private Coroutine laserRoutine;

    [Header("Clon Config")]
    [SerializeField] private GameObject clonPrefab;
    [SerializeField] private Transform clonSpawnPoint;
    [SerializeField] private float cooldown;

    private GameObject currentAbility2;
    private bool ability2OnCooldown = false;

    private GameObject currentLaser;
    private bool isUsingLaser = false;
    public Vector2 aimInput;

    public bool IsUsingLaser()
    {
        return isUsingLaser;
    }


    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!IsActive) return;

        if (context.performed && playerController.IsGrounded)
        {
            StartChargingLaser();
        }
        else if (context.canceled)
        {
            StopLaser();
        }
    }

    private void Update()
    {
        if (!IsActive) return;

        if (!playerController.IsGrounded)
        {
            StopLaser();
            return;
        }

        if (isUsingLaser)
        {
            HandleLaserRotation();
        }
    }

    // ---------------- LOGICA LASER ---------------- //

    private void StartLaser()
    {
        if (isUsingLaser) return;

        aimPivot.localRotation = Quaternion.Euler(0, 0, 0);

        isUsingLaser = true;

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
        animator.SetBool("IsUsingLaser", false);

        playerController.movementLocked = false;

        if (laserRoutine != null)
        {
            StopCoroutine(laserRoutine);
            laserRoutine = null;
        }

        if (!isUsingLaser) return;

        isUsingLaser = false;

        if (currentLaser != null)
        {
            currentLaser.SetActive(false);
        }
    }

    private void HandleLaserRotation()
    {
        Vector2 moveInput = playerController.GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(moveInput.y, 0, -moveInput.x).normalized;

        if (inputDir.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,rotationSpeed * Time.deltaTime / rotationSmoothTime);
        }

        float vertical = aimInput.y;
        float currentX = aimPivot.localEulerAngles.x;

        if (currentX > 180) currentX -= 360;

        currentX = Mathf.Clamp(currentX - vertical * verticalAimSpeed * Time.deltaTime, -45f, 45f);

        aimPivot.localRotation = Quaternion.Euler(currentX, 0, 0);

        if (currentLaser != null)
        {
            currentLaser.transform.position = spawnPoint.position;
            currentLaser.transform.rotation = aimPivot.rotation;
        }
    }

    public void AimVertical(InputAction.CallbackContext context)
    {
        if (!IsActive) return;
        aimInput = context.ReadValue<Vector2>();
    }

        private void StartChargingLaser()
    {
        if (isUsingLaser || laserRoutine != null) return;

        playerController.movementLocked = true;

        animator.SetBool("IsUsingLaser", true);

        laserRoutine = StartCoroutine(LaserStartup());
    }

        private IEnumerator LaserStartup()
    {
        yield return new WaitForSeconds(delay);

        StartLaser();

        laserRoutine = null;
    }

    // ---------------- LOGICA CLON ---------------- //

    public void Habilidad2(InputAction.CallbackContext context)
    {
        if (!IsActive) return;

        if (context.performed && CanUseAbility2())
        {
            ActivateAbility2();
        }
    }

    private bool CanUseAbility2()
    {
        return currentAbility2 == null && !ability2OnCooldown;
    }

    private void ActivateAbility2()
    {
        currentAbility2 = Instantiate(clonPrefab, clonSpawnPoint.position, clonSpawnPoint.rotation);

        ClonInstance instance = currentAbility2.GetComponent<ClonInstance>();

        if (instance != null)
        {
            instance.Init(OnAbility2Destroyed);
        }

        print("Habilidad 2 activada");
    }

    private void OnAbility2Destroyed()
    {
        currentAbility2 = null;
        StartCoroutine(Ability2Cooldown());
    }

    private IEnumerator Ability2Cooldown()
    {
        ability2OnCooldown = true;

        yield return new WaitForSeconds(cooldown);

        ability2OnCooldown = false;

        print("Habilidad 2 lista otra vez");
    }
}
