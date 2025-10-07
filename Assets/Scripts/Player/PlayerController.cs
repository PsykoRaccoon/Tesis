using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] public float walkSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField, Range(0f, 1f)] public float airControl;
    private float currentSpeed;

    [Header("Salto")]
    [SerializeField] public float jumpHeight;
    [SerializeField] private float gravity;

    [Header("Rotaci�n y Apuntado")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private LineRenderer laserLine;

    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;
    private Vector3 moveDirection;

    private bool runToggle = false;
    private bool isAiming = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;

        if (laserLine != null)
        {
            laserLine.positionCount = 2; // inicio y fin del rayo
        }
    }

    private void Update()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (!isAiming)
        {
            // Movimiento normal
            if (inputDir.magnitude > 0)
            {
                currentSpeed = runToggle ? runSpeed : walkSpeed;
                moveDirection = inputDir;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime / rotationSmoothTime
                );
            }
            else
            {
                if (!controller.isGrounded)
                {
                    moveDirection *= airControl;
                }
                else
                {
                    currentSpeed = 0f;
                    runToggle = false;
                    moveDirection = Vector3.zero;
                }
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            // Solo rotaci�n (sin movimiento)
            if (inputDir.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime / rotationSmoothTime
                );
            }
        }

        // Gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Dibujar l�ser
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        if (laserLine == null || laserOrigin == null) return;

        Vector3 start = laserOrigin.position;
        Vector3 end = start + transform.forward * 10f; // largo del l�ser

        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
    }

    // ----------------- INPUTS -----------------
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            runToggle = !runToggle;
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;
        }
        else if (context.canceled)
        {
            isAiming = false;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}
