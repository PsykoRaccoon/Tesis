using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] public float walkSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField, Range(0f, 1f)] public float airControl; //momentum
    private float currentSpeed; 

    [Header("Salto")]
    [SerializeField] public float jumpHeight;
    [SerializeField] private float gravity;

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
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

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
}
