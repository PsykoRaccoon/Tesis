using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityManager : MonoBehaviour
{
    [Header("Scripts de habilidades")]
    [SerializeField] private MonoBehaviour scriptHabilidadesFuego;
    [SerializeField] private MonoBehaviour scriptHabilidadesTierra;

    private FireAbilities fuego;
    private EarthAbilities tierra;
    private bool abilityAIsActive = true;

    private void Awake()
    {
        fuego = scriptHabilidadesFuego as FireAbilities;
        tierra = scriptHabilidadesTierra as EarthAbilities;

        fuego.IsActive = true;
        tierra.IsActive = false;
    }

    public void SwitchAbility(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (abilityAIsActive && fuego != null && fuego.IsUsingAbility())
        {
            Debug.Log("No se puede cambiar: habilidad 1 en uso.");
            return;
        }

        abilityAIsActive = !abilityAIsActive;

        fuego.IsActive = abilityAIsActive;
        tierra.IsActive = !abilityAIsActive;

        if (abilityAIsActive)
        {
            Debug.Log("HAB 1 ON, HAB 2 OFF");
        }
        else
        {
            Debug.Log("HAB 2 ON, HAB 1");
        }
    }
}
