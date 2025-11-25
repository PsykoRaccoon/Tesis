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

    [Header("Rotación")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothTime;

    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;
    private Vector3 moveDirection;

    private bool runToggle = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        // Safety Check en lo que queda el otro personaje para no llenar la consola de errores :D
        if (animator == null || controller == null)
        {
            return;
        }

        animator.SetBool("IsGrounded", controller.isGrounded);

        if (movementLocked)
        {
            moveDirection = Vector3.zero;
            currentSpeed = 0f;
            velocity = new Vector3(0, Mathf.Clamp(velocity.y, -2f, float.MaxValue), 0);
            controller.Move(velocity * Time.deltaTime);
            animator.SetFloat("Velocity", 0f, 0.1f, Time.deltaTime);
            return;
        }

        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        float targetAnimationSpeed = 0f;

        if (inputDir.magnitude > 0)
        {
            currentSpeed = runToggle ? runSpeed : walkSpeed;
            moveDirection = inputDir;
            targetAnimationSpeed = currentSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / rotationSmoothTime);
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

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        animator.SetFloat("Velocity", targetAnimationSpeed, 0.1f, Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

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
}