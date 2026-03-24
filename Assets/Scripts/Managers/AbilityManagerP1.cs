using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManagerP1 : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private WaterAbilities water;
    [SerializeField] private AirAbilities air;

    private bool isWaterActive = true;

    private void Start()
    {
        ActivateWater();
    }

    public void SwitchAbility(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isWaterActive && water != null && water.IsUsingLaser())
        {
            Debug.Log("No puedes cambiar: laser activo");
            return;
        }

        isWaterActive = !isWaterActive;

        if (isWaterActive)
        {
            ActivateWater();
            Debug.Log("AGUA ON | AIRE OFF");
        }
        else
        {
            ActivateAir();
            Debug.Log("AIRE ON | AGUA OFF");
        }
    }

    private void ActivateWater()
    {
        if (water != null)
        {
            water.IsActive = true;
        }

        if (air != null)
        {
            air.IsActive = false;
        }
            
    }

    private void ActivateAir()
    {
        if (water != null)
            water.IsActive = false;

        if (air != null)
            air.IsActive = true;
    }
}