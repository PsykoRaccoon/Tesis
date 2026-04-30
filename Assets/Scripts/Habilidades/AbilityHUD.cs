using UnityEngine;

public class AbilityHUD : MonoBehaviour
{
    public static AbilityHUD Instance { get; private set; }

    [Header("Tierra")]
    public CooldownUI earthBlockIcon;
    public CooldownUI earthRampIcon;

    [Header("Fuego")]
    public CooldownUI fireballIcon;
    public CooldownUI laserIcon;

    [Header("Agua")]
    public CooldownUI waterLaserIcon;
    public CooldownUI waterClonIcon;

    [Header("Aire")]
    public CooldownUI airRepulsionIcon;
    public CooldownUI airAttractionIcon;

    private void Awake()
    {
        Instance = this;
    }
}