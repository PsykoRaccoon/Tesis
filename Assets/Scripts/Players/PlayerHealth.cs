using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Feedback")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration;

    [Header("Stun")]
    [SerializeField] private float stunDuration;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string damageTriggerName;

    private Color originalColor;
    private bool isStunned = false;

    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();

        playerController = GetComponent<PlayerController>();

        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }

    public override void TakeDamage(int amount, DamageType type)
    {
        TakeDamage(amount, type, transform.position);
    }

    public override void TakeDamage(int amount, DamageType type, Vector3 sourcePosition)
    {
        if (isDead) return;

        Vector3 knockbackDirection = (transform.position - sourcePosition).normalized;

        if (knockbackDirection == Vector3.zero)
            knockbackDirection = -transform.forward;

        if (playerController != null)
            playerController.ApplyKnockback(knockbackDirection * knockbackForce);

        if (animator != null)
            animator.SetTrigger(damageTriggerName);

        StartCoroutine(DamageFlash());

        if (type == DamageType.Player)
        {
            currentHealth -= amount;
            if (currentHealth <= 1) currentHealth = 1;
            Debug.Log("Friendly Fire!");
            StartCoroutine(Stun());
            return;
        }

        base.TakeDamage(amount, type);
    }

    // ---------------- FEEDBACK ---------------- //

    private System.Collections.IEnumerator DamageFlash()
    {
        if (playerRenderer == null) yield break;

        int flashes = 3;

        for (int i = 0; i < flashes; i++)
        {
            playerRenderer.material.color = damageColor;
            yield return new WaitForSeconds(0.05f);

            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // ---------------- STUN ---------------- //

    private System.Collections.IEnumerator Stun()
    {
        if (isStunned) yield break;

        isStunned = true;

        if (playerController != null)
            playerController.movementLocked = true;

        yield return new WaitForSeconds(stunDuration);

        if (playerController != null)
            playerController.movementLocked = false;

        isStunned = false;
    }

    // ---------------- MUERTE ---------------- //

    protected override void Die(DamageType type)
    {
        isDead = true;
        Debug.Log("Jugador murió por: " + type);
        gameObject.SetActive(false);
    }
}