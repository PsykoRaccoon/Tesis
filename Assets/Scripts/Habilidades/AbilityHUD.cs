using UnityEngine;

/// <summary>
/// Ponlo en el Canvas de la escena. Tiene las referencias a los 4 íconos.
/// Los scripts del prefab lo buscan solos en Start(), sin arrastrar nada.
/// </summary>
public class AbilityHUD : MonoBehaviour
{
    public static AbilityHUD Instance { get; private set; }

    [Header("Tierra")]
    public CooldownUI earthBlockIcon;
    public CooldownUI earthRampIcon;

    [Header("Fuego")]
    public CooldownUI fireballIcon;
    public CooldownUI laserIcon;

    private void Awake()
    {
        Instance = this;
    }
}