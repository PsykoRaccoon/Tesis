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

    private void Awake()
    {
        _a = abilityA as IAbility;
        _b = abilityB as IAbility;

        if (_a != null) _a.IsActive = true;
        if (_b != null) _b.IsActive = false;
    }

    public void UnlockAbilityB()
    {
        Debug.Log("Habilidad B desbloqueada");
    }

    public void UnlockAndKeepCurrent()
    {
        if (_b != null) _b.IsActive = false; 
        Debug.Log("Habilidad B desbloqueada");
    }

    public void SwitchAbility(InputAction.CallbackContext context)
    {
        Debug.Log($"SwitchAbility llamado | enabled: {enabled} | performed: {context.performed}");
        
        if (!enabled || !context.performed) return;

        Debug.Log($"isAActive: {isAActive} | _a null: {_a == null} | _b null: {_b == null}");

        if (isAActive && _a != null && _a.IsUsingAbility())
        {
            Debug.Log($"Bloqueado por IsUsingAbility: {_a.IsUsingAbility()}");
            return;
        }

        isAActive = !isAActive;
        if (_a != null) _a.IsActive = isAActive;
        if (_b != null) _b.IsActive = !isAActive;

        Debug.Log($"Después del switch → A.IsActive: {_a?.IsActive} | B.IsActive: {_b?.IsActive}");
    }
}