using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    [Header("Habilidades")]
    [SerializeField] private MonoBehaviour abilityA;
    [SerializeField] private MonoBehaviour abilityB;

    private IAbility _a;
    private IAbility _b;
    private bool isAActive = true;
    private bool isBUnlocked = false; // <- la clave

    private void Awake()
    {
        _a = abilityA as IAbility;
        _b = abilityB as IAbility;

        if (_a != null) _a.IsActive = true;
        if (_b != null) _b.IsActive = false;
    }

    public void UnlockAndKeepCurrent()
    {
        isBUnlocked = true;
        if (_b != null) _b.IsActive = false;
        Debug.Log("Habilidad B desbloqueada");
    }

    public void SwitchAbility(InputAction.CallbackContext context)
    {
        if (!enabled || !context.performed) return;

        if (!isBUnlocked)
        {
            Debug.Log("Habilidad B todavía no desbloqueada");
            return;
        }

        if (isAActive && _a != null && _a.IsUsingAbility())
        {
            Debug.Log("Bloqueado: habilidad en uso");
            return;
        }

        isAActive = !isAActive;
        if (_a != null) _a.IsActive = isAActive;
        if (_b != null) _b.IsActive = !isAActive;

        Debug.Log(isAActive ? "A ON | B OFF" : "B ON | A OFF");
    }
}