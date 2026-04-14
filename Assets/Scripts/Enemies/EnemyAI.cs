using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Transform player;

    // NUEVO 1: Variable para guardar el script de vida del jugador
    private VidaJugadorUI scriptVidaJugador;

    [Header("Configuración de Distancias")]
    [Tooltip("El enemigo despierta si el jugador entra en este radio.")]
    public float detectionRadius = 10f;
    [Tooltip("Si el jugador entra en este radio, cuenta como que lo tocó.")]
    public float touchRadius = 1.2f;

    [Header("Mecánica de Sombra Básica (Tesis)")]
    public float cooldownEntreToques = 2f;

    private int cantidadDeToques = 0;
    private float tiempoUltimoToque = -10f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
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

                // NUEVO 2: Cuando encontramos al jugador, también extraemos su script de UI
                scriptVidaJugador = playerObject.GetComponent<VidaJugadorUI>();
            }
            return;
        }

        // 2. Medimos la distancia exacta al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 3. LÓGICA DE MOVIMIENTO
        if (distanceToPlayer <= detectionRadius)
        {
            if (navAgent.isActiveAndEnabled && cantidadDeToques < 2)
            {
                navAgent.SetDestination(player.position);
            }
        }
        else
        {
            if (navAgent.isActiveAndEnabled && navAgent.hasPath)
            {
                navAgent.ResetPath();
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

        // NUEVO 3: ¡Le avisamos al jugador que cambie su imagen!
        if (scriptVidaJugador != null)
        {
            scriptVidaJugador.RecibirToqueSombra();
        }

        if (cantidadDeToques == 1)
        {
            Debug.Log("La IA registró el 1er toque internamente.");
        }
        else if (cantidadDeToques >= 2)
        {
            Debug.Log("La IA registró el 2do toque y se detiene.");
            navAgent.isStopped = true;
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