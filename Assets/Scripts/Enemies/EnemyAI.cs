using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Transform player;

    [Header("Configuración de Distancias")]
    [Tooltip("El enemigo despierta si el jugador entra en este radio.")]
    public float detectionRadius = 10f;
    [Tooltip("Si el jugador entra en este radio, cuenta como que lo tocó.")]
    public float touchRadius = 1.2f;

    [Header("Mecánica de Sombra Básica")]
    public float cooldownEntreToques = 2f;

    private int cantidadDeToques = 0;
    private float tiempoUltimoToque = -10f;

    // --- VARIABLES DE DIAGNÓSTICO (Para no saturar la consola) ---
    private bool logJugadorEncontrado = false;
    private bool logPersiguiendo = false;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        // DEBUG 1: ¿El enemigo está tocando el suelo horneado (Bake)?
        if (navAgent.isOnNavMesh)
        {
            Debug.Log("[DIAGNÓSTICO] Todo bien. La Sombra Básica está tocando el NavMesh.");
        }
        else
        {
            Debug.LogError("[ERROR CRÍTICO] La Sombra Básica NO está sobre el NavMesh. ¡Por eso no se mueve! Revisa su altura en el eje Y o vuelve a darle 'Bake' al suelo de este mapa.");
        }
    }

    void Update()
    {
        // 1. Buscamos al jugador si aún no spawnea
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;

                // DEBUG 2: Confirmar que el spawner funcionó y el Tag es correcto
                if (!logJugadorEncontrado)
                {
                    Debug.Log("[DIAGNÓSTICO] ¡Jugador encontrado exitosamente en la escena!");
                    logJugadorEncontrado = true;
                }
            }
            return;
        }

        // 2. Medimos la distancia exacta al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ¡AÑADE ESTA LÍNEA AQUÍ PARA VER LA VERDAD!
        Debug.Log($"[RADAR] Distancia al jugador: {distanceToPlayer}. Mi radio es: {detectionRadius}");

        // 3. LÓGICA DE MOVIMIENTO
        if (distanceToPlayer <= detectionRadius)
        {
            if (navAgent.isActiveAndEnabled && cantidadDeToques < 2)
            {
                navAgent.SetDestination(player.position);

                // DEBUG 3: Confirmar que la orden de persecución se está enviando
                if (!logPersiguiendo)
                {
                    Debug.Log($"🏃 [DIAGNÓSTICO] El jugador entró al radio. Perseguido... ¿Hay ruta disponible?: {navAgent.hasPath}");
                    logPersiguiendo = true;
                }
            }
        }
        else
        {
            if (navAgent.isActiveAndEnabled && navAgent.hasPath)
            {
                navAgent.ResetPath();

                // DEBUG 4: Confirmar que se detuvo correctamente
                if (logPersiguiendo)
                {
                    Debug.Log("[DIAGNÓSTICO] El jugador salió del radio de visión. Sombra detenida.");
                    logPersiguiendo = false;
                }
            }
        }

        // 4. LÓGICA DE TOQUE (Matemática pura, sin físicas)
        if (distanceToPlayer <= touchRadius)
        {
            // Verificamos el tiempo de inmunidad
            if (Time.time >= tiempoUltimoToque + cooldownEntreToques)
            {
                AplicarEfectoSombra();
            }
        }
    }

    void AplicarEfectoSombra()
    {
        cantidadDeToques++;
        tiempoUltimoToque = Time.time;

        if (cantidadDeToques == 1)
        {
            Debug.Log("ESTADO: Contaminado. El jugador ha sido tocado por primera vez. (Bloquear cambio de elemento)");
        }
        else if (cantidadDeToques == 2)
        {
            Debug.Log("ESTADO: Sombrificado. El jugador ha sido tocado por segunda vez. (Muerte del jugador)");
            navAgent.isStopped = true; // El enemigo se detiene al matar al jugador
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, touchRadius);
    }
}