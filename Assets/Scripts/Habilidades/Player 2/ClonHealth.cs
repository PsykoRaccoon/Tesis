using UnityEngine;

public class ClonHealth : MonoBehaviour, IDamageable
{
    private bool isDead = false;

    public void TakeDamage(int amount)
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