using UnityEngine;

public class PlayerHealth : Health
{
    public override void TakeDamage(int amount, DamageType type)
    {
        if (isDead) return;

        if (type == DamageType.Player)
        {
            currentHealth -= amount;

            if (currentHealth <= 1)
                currentHealth = 1;

            Debug.Log("Friendly Fire|");

            return;
        }

        base.TakeDamage(amount, type);
    }

    protected override void Die(DamageType type)
    {
        isDead = true;

        Debug.Log("Jugador murió por: " + type);

        // logica de muerte como la animacion

        gameObject.SetActive(false); 
    }
}