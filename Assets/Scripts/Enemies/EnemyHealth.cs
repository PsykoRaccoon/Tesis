using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    public bool IsDead { get; private set; } = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        Destroy(gameObject); // de momento destrucción directa
    }
}
