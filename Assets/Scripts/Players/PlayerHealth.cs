using UnityEngine;
using UnityEngine.UI;
using TMPro; // Librería de TextMeshPro

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

    [Header("Respawn y UI de Muerte")]
    [Tooltip("Nombre EXACTO del Panel de Muerte en el Canvas (ej. PanelMuerteP1)")]
    public string nombrePanelMuerte = "PanelMuerteP1";
    [Tooltip("Nombre EXACTO del Texto dentro de ese panel (ej. TextoConteoP1)")]
    public string nombreTextoConteo = "TextoConteoP1";
    public float tiempoRespawn = 5f;

    private GameObject panelMuerte;
    private TextMeshProUGUI textoConteo;
    private Vector3 puntoDeRespawn;

    private Color originalColor;
    private bool isStunned = false;

    private PlayerController playerController;
    private Collider[] colisionadores;

    protected override void Awake()
    {
        base.Awake();

        playerController = GetComponent<PlayerController>();
        colisionadores = GetComponentsInChildren<Collider>();

        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }

    private void Start()
    {
        // 1. Buscamos el Canvas principal en la escena
        Canvas canvasPrincipal = FindObjectOfType<Canvas>();

        if (canvasPrincipal != null)
        {
            // BÚSQUEDA PROFUNDA: Escaneamos absolutamente todo dentro del Canvas (incluyendo objetos apagados)
            Transform[] todosLosObjetosUI = canvasPrincipal.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in todosLosObjetosUI)
            {
                // Si encontramos el panel por su nombre
                if (t.name == nombrePanelMuerte)
                {
                    panelMuerte = t.gameObject;

                    // Inmediatamente buscamos el texto del conteo dentro de este panel
                    Transform tTexto = t.Find(nombreTextoConteo);
                    if (tTexto != null)
                    {
                        textoConteo = tTexto.GetComponent<TextMeshProUGUI>();
                    }

                    break; // Ya lo encontramos, salimos del ciclo
                }
            }
        }

        // Validación de seguridad por si acaso
        if (panelMuerte == null)
        {
            Debug.LogError($"🚨 ERROR CRÍTICO: El script no pudo encontrar el panel '{nombrePanelMuerte}' en ninguna parte del Canvas. Revisa que el nombre esté bien escrito.");
        }

        // Guardamos la posición inicial para el respawn
        Invoke("GuardarPuntoRespawn", 0.1f);
    }

    private void GuardarPuntoRespawn()
    {
        puntoDeRespawn = transform.position;
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
            StartCoroutine(Stun());
            return;
        }

        base.TakeDamage(amount, type);
    }

    // ---------------- FEEDBACK Y STUN ---------------- //

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

    // ---------------- MUERTE Y RESPAWN ---------------- //

    protected override void Die(DamageType type)
    {
        isDead = true;
        Debug.Log("Jugador murió por: " + type);

        StartCoroutine(RutinaDeMuerteYRespawn());
    }

    private System.Collections.IEnumerator RutinaDeMuerteYRespawn()
    {
        // 1. SOLUCIÓN AL ERROR: Desactivamos por completo el SCRIPT de movimiento
        if (playerController != null) playerController.enabled = false;

        // 2. Escondemos el modelo y apagamos colisiones físicas
        if (playerRenderer != null) playerRenderer.enabled = false;
        foreach (Collider col in colisionadores) col.enabled = false;

        // 3. Encendemos su pantalla de muerte (ahora que la referencia sí existe)
        if (panelMuerte != null) panelMuerte.SetActive(true);

        // 4. Conteo Regresivo
        float tiempoRestante = tiempoRespawn;
        while (tiempoRestante > 0)
        {
            if (textoConteo != null)
            {
                textoConteo.text = "Reapareciendo en: " + Mathf.CeilToInt(tiempoRestante).ToString();
            }
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
        }

        // 5. Apagamos el menú de muerte
        if (panelMuerte != null) panelMuerte.SetActive(false);

        // 6. Teletransportación y restauración de estadísticas
        transform.position = puntoDeRespawn;
        currentHealth = 100; // Ajusta esto a tu vida máxima nativa de la clase Health
        isDead = false;

        // 7. Devolvemos el control y visibilidad al jugador
        if (playerRenderer != null) playerRenderer.enabled = true;
        foreach (Collider col in colisionadores) col.enabled = true;

        // Reactivamos el script de movimiento para que vuelva a escuchar al control
        if (playerController != null) playerController.enabled = true;

        Debug.Log("¡Jugador Resucitado e interfaz limpia!");
    }
}