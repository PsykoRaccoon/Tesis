using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorActivator : MonoBehaviour
{
    [SerializeField] private Elevator elevator;

    private PlayerInput currentPlayer;
    private InputAction interactAction;
    private bool isUnlocked = false;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void Unlock()
    {
        isUnlocked = true;
        CheckForPlayersAlreadyInside();
    }

    public void Lock()
    {
        isUnlocked = false;
        UnsubscribeCurrentPlayer();
    }

    private void CheckForPlayersAlreadyInside()
    {
        Vector3 center = transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size / 2;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation);
        foreach (var hit in hits)
        {
            PlayerInput player = hit.GetComponent<PlayerInput>();
            if (player == null) continue;

            SubscribePlayer(player);
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentPlayer != null) return;
        if (!isUnlocked) return;

        PlayerInput player = other.GetComponent<PlayerInput>();
        if (player == null) return;

        SubscribePlayer(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentPlayer == null) return;

        PlayerInput player = other.GetComponent<PlayerInput>();
        if (player != currentPlayer) return;

        UnsubscribeCurrentPlayer();
    }

    private void SubscribePlayer(PlayerInput player)
    {
        currentPlayer = player;
        interactAction = currentPlayer.actions["Interact"];
        interactAction.performed += OnInteract;
    }

    private void UnsubscribeCurrentPlayer()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;

        interactAction = null;
        currentPlayer = null;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        elevator.CycleElement();
    }

    private void OnDestroy()
    {
        UnsubscribeCurrentPlayer();
    }
}