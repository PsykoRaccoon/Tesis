using UnityEngine;
using UnityEngine.InputSystem;

public class WaterAbilities : MonoBehaviour
{
    public bool IsActive { get; set; } = true;

    [Header("Referencias")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private GameObject laserBeamPrefab;

    [Header("Configuración")]
    [SerializeField] private float verticalAimSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0.05f, 1f)] private float rotationSmoothTime;

    private GameObject currentLaser;
    private bool isUsingLaser = false;
    private Vector2 aimInput;


    public void Habilidad1(InputAction.CallbackContext context)
    {
        if (!IsActive) return;

        if (context.performed && playerController.IsGrounded)
        {
            StartLaser();
            print("Rasho laser activado");
        }
        else if (context.canceled && isUsingLaser)
        {
            StopLaser();
            print("Rasho laser desactivado");
        }
    }

    public void AimVertical(InputAction.CallbackContext context)
    {
        if (!IsActive) return;
        aimInput = context.ReadValue<Vector2>();
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
        if (!isUsingLaser) return;

        isUsingLaser = false;
        playerController.movementLocked = false;

        if (currentLaser != null)
        {
            currentLaser.SetActive(false);
        }
    }

    private void HandleLaserRotation()
    {
        Vector2 moveInput = playerController.GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Rotation horizontal del player
        if (inputDir.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime / rotationSmoothTime
            );
        }

        // Rotation vertical del laser
        float vertical = aimInput.y;
        float currentX = aimPivot.localEulerAngles.x;

        if (currentX > 180) currentX -= 360;

        currentX = Mathf.Clamp(
            currentX - vertical * verticalAimSpeed * Time.deltaTime,
            -45f,
            45f
        );

        aimPivot.localRotation = Quaternion.Euler(currentX, 0, 0);

        // Posicionar laser
        if (currentLaser != null)
        {
            currentLaser.transform.position = spawnPoint.position;
            currentLaser.transform.rotation = aimPivot.rotation;
        }
    }
}
