using UnityEngine;

public class ClonHealth : MonoBehaviour, IDamageable
{
    private bool isDead = false;

    public void TakeDamage(int amount, DamageType type)
    {
        if (isDead) return;

        Die();
    }

    private void Die()
    {
        isDead = true;

        Destroy(gameObject);
    }
}