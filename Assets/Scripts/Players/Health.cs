using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth = 10;

    protected int currentHealth;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount, DamageType type)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die(type);
    }

    public virtual void TakeDamage(int amount, DamageType type, Vector3 sourcePosition)
    {
        TakeDamage(amount, type);
    }

    protected virtual void Die(DamageType type)
    {
        isDead = true;
        Destroy(gameObject);
    }
}