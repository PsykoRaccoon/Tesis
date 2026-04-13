using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    [SerializeField] public float walkSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField] public bool movementLocked;
    [SerializeField, Range(0f, 1f)] public float airControl;
    private float currentSpeed;

    [Header("Salto")]
    [SerializeField] public float jumpHeight;
    [SerializeField] private float gravity = -9.81f;

    [Header("Rotacion")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothTime;

    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;
    private Vector3 moveDirection;

    private Vector3 externalVelocity;

    private bool runToggle = false;

    public bool IsGrounded => controller != null && controller.isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        if (animator == null || controller == null) return;

        animator.SetBool("IsGrounded", controller.isGrounded);

        if (movementLocked)
        {
            moveDirection = Vector3.zero;
            currentSpeed = 0f;
        }
        else
        {
            Vector3 inputDir = new Vector3(moveInput.y, 0, -moveInput.x).normalized;
            float targetAnimationSpeed = 0f;

            if (inputDir.magnitude > 0)
            {
                currentSpeed = runToggle ? runSpeed : walkSpeed;
                moveDirection = inputDir;
                targetAnimationSpeed = currentSpeed;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime / rotationSmoothTime
                );
            }
            else
            {
                targetAnimationSpeed = 0f;

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

            animator.SetFloat("Velocity", targetAnimationSpeed, 0.1f, Time.deltaTime);
        }

        Vector3 totalMove = moveDirection * currentSpeed + externalVelocity;
        controller.Move(totalMove * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 10f * Time.deltaTime);
    }

    // ---------------- INPUT ---------------- //

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (movementLocked) return;

        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            runToggle = !runToggle;
        }
    }

    // ---------------- KNOCKBACK ---------------- //

    public void ApplyKnockback(Vector3 force)
    {
        externalVelocity = force;
    }
}