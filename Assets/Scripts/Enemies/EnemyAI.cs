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

    [Header("Mecánica de Sombra")]
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
            if (playerObject != null) player = playerObject.transform;
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

        // 4. NUEVA LÓGICA DE TOQUE (Matemática pura, sin físicas)
        if (distanceToPlayer <= touchRadius)
        {
            // Verificamos el tiempo de inmunidad
            if (Time.time >= tiempoUltimoToque + cooldownEntreToques)
            {
                AplicarEfectoSombra();
            }
        }
    }

    // Separamos la lógica de tu tesis en su propia función para mantener todo ordenado
    void AplicarEfectoSombra()
    {
        cantidadDeToques++;
        tiempoUltimoToque = Time.time;

        if (cantidadDeToques == 1)
        {
            Debug.Log("⚠️ ESTADO: Contaminado. El jugador ha sido tocado por primera vez. (Bloquear cambio de elemento)");
        }
        else if (cantidadDeToques == 2)
        {
            Debug.Log("☠️ ESTADO: Sombrificado. El jugador ha sido tocado por segunda vez. (Muerte del jugador)");
            navAgent.isStopped = true; // El enemigo se detiene al matar al jugador
        }
    }

    // Dibujamos ambos radios en el editor para que puedas ajustarlos visualmente
    void OnDrawGizmosSelected()
    {
        // Esfera Roja = Rango de visión
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Esfera Amarilla = Rango de Toque (Daño)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, touchRadius);
    }
}