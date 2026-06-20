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

    [Header("Respawn")]
    public float tiempoRespawn;
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject[] renderersToDisable;

    private Color originalColor;
    private bool isStunned = false;

    private PlayerController playerController;
    private Collider[] colisionadores;
    private SpawnManager spawnManager;

    protected override void Awake()
    {
        base.Awake();
        playerController = GetComponent<PlayerController>();
        colisionadores = GetComponentsInChildren<Collider>();
        spawnManager = FindObjectOfType<SpawnManager>();

        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;
    }

    private void Start()
    {
        PlayerUIManager.Instance.ActualizarEstadoVida(playerIndex, currentHealth);
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

        if (playerController != null && playerController.enabled)
            playerController.ApplyKnockback(knockbackDirection * knockbackForce);

        if (animator != null)
            animator.SetTrigger(damageTriggerName);

        StartCoroutine(DamageFlash());

        if (type == DamageType.Player)
        {
            currentHealth -= amount;
            if (currentHealth <= 1) currentHealth = 1;
            Debug.Log("Friendly Fire!");
            PlayerUIManager.Instance.ActualizarEstadoVida(playerIndex, currentHealth);
            StartCoroutine(Stun());
            return;
        }

        base.TakeDamage(amount, type);
        PlayerUIManager.Instance.ActualizarEstadoVida(playerIndex, currentHealth);
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        if (playerRenderer == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            playerRenderer.material.color = damageColor;
            yield return new WaitForSeconds(0.05f);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

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

    protected override void Die(DamageType type)
    {
        isDead = true;
        Debug.Log("Jugador " + playerIndex + " murió por: " + type);
        StartCoroutine(RutinaDeMuerteYRespawn());
    }

    private System.Collections.IEnumerator RutinaDeMuerteYRespawn()
    {
        if (playerController != null) playerController.enabled = false;
        if (playerRenderer != null) playerRenderer.enabled = false;
        foreach (Collider col in colisionadores) col.enabled = false;
        foreach (GameObject go in renderersToDisable) go.SetActive(false);

        PlayerUIManager.Instance.ShowDeathPanel(playerIndex);

        float tiempoRestante = tiempoRespawn;
        while (tiempoRestante > 0)
        {
            PlayerUIManager.Instance.UpdateCountdownText(playerIndex, tiempoRestante);
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
        }

        PlayerUIManager.Instance.HideDeathPanel(playerIndex);

        transform.position = spawnManager.GetRespawnPosition(playerIndex);
        currentHealth = maxHealth;
        isDead = false;

        PlayerUIManager.Instance.ActualizarEstadoVida(playerIndex, currentHealth);

        if (playerRenderer != null) playerRenderer.enabled = true;
        foreach (Collider col in colisionadores) col.enabled = true;
        foreach (GameObject go in renderersToDisable) go.SetActive(true);
        if (playerController != null) playerController.enabled = true;

        Debug.Log("¡Jugador " + playerIndex + " resucitado!");
    }
}