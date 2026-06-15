using UnityEngine;

public class ProjectileVFX : MonoBehaviour
{
    [SerializeField] private GameObject impactVFXPrefab;
    private AbilityProjectile projectile;

    private void Awake()
    {
        projectile = GetComponent<AbilityProjectile>();
        projectile.OnImpact += SpawnVFX;
    }

    private void SpawnVFX(Vector3 position)
    {
        if (impactVFXPrefab != null)
            Instantiate(impactVFXPrefab, position, Quaternion.identity);
    }
}