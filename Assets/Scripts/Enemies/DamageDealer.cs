using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount;
    [SerializeField] private DamageType damageType; //quien esta haciendo el damage

    [Tooltip("Tags que puede lastimar")]
    [SerializeField] private string[] targetTags; //a quien puede lastimar

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidTarget(other)) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount, damageType);
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