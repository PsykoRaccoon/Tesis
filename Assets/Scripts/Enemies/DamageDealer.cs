using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount;

    [Tooltip("Tags que este objeto puede dañar")]
    [SerializeField] private string[] targetTags;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidTarget(other)) return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }
    }

    private bool IsValidTarget(Collider other)
    {
        foreach (string tag in targetTags)
        {
            if (other.CompareTag(tag))
                return true;
        }

        return false;
    }
}