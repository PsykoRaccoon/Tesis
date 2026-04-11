using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private float stunDuration;

    private bool isStunned = false;
    public override void TakeDamage(int amount, DamageType type)
    {
        if (isDead) return;

        if (type == DamageType.Player)
        {
            currentHealth -= amount;

            if (currentHealth <= 1)
                currentHealth = 1;

            Debug.Log("Friendly Fire!");

            StartCoroutine(Stun()); 

            return;
        }

        base.TakeDamage(amount, type);
    }

    private System.Collections.IEnumerator Stun()
    {
        if (isStunned) yield break;

        isStunned = true;

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.movementLocked = true;
        }

        yield return new WaitForSeconds(stunDuration);

        if (controller != null)
        {
            controller.movementLocked = false;
        }

        isStunned = false;
    }

    protected override void Die(DamageType type)
    {
        isDead = true;

        Debug.Log("Jugador murió por: " + type);

        // logica de muerte como la animacion

        gameObject.SetActive(false); 
    }
}