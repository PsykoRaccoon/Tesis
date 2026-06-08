using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Animator animator;
    private PlayerHealth playerHealth; // Reemplaza VidaJugadorUI

    private Transform player;
    private GameObject[] allPlayers;

    [Header("Distancias")]
    public float detectionRadius;
    public float touchRadius;

    [Header("Daño")]
    public float cooldownEntreToques;
    [SerializeField] private int dañoPorToque;

    [Header("Merodeo")]
    public float wanderRadius;
    public float minWaitTime;
    public float maxWaitTime;
    public float stuckDistanceThreshold;
    public float stuckTimeLimit;

    private static readonly int StateParam = Animator.StringToHash("State");
    private const int STATE_IDLE      = 0;
    private const int STATE_WALKING   = 1;
    private const int STATE_ATTACKING = 2;
    private const int STATE_DEAD      = 3;

    private float tiempoUltimoToque;
    private Vector3 spawnPosition;
    private EnemyHealth enemyHealth;

    private enum WanderState { Waiting, Moving }
    private WanderState wanderState = WanderState.Waiting;
    private float wanderWaitTimer;
    private float stuckTimer;
    private float lastRemainingDistance;

    void Start()
    {
        navAgent      = GetComponent<NavMeshAgent>();
        animator      = GetComponent<Animator>();
        enemyHealth   = GetComponent<EnemyHealth>();
        spawnPosition = transform.position;
        allPlayers    = new GameObject[0];

        StartWaitTimer();
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead) return;

        UpdateNearestPlayer();

        if (player == null)
        {
            HandleWander();
            HandleAnimations(Mathf.Infinity);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            wanderState = WanderState.Waiting;
            HandleMovement(distanceToPlayer);
            HandleDamage(distanceToPlayer);
        }
        else
        {
            HandleWander();
        }

        HandleAnimations(distanceToPlayer);
    }

    void UpdateNearestPlayer()
    {
        if (allPlayers == null || allPlayers.Length == 0)
        {
            allPlayers = GameObject.FindGameObjectsWithTag("Player");
            if (allPlayers.Length == 0) return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject p in allPlayers)
        {
            if (p == null || !p.activeInHierarchy) continue;

            PlayerHealth ph = p.GetComponent<PlayerHealth>();
            if (ph != null && ph.IsDead) continue;

            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestPlayer = p.transform;
            }
        }

        if (closestPlayer != null && closestPlayer != player)
        {
            player = closestPlayer;
            playerHealth = player.GetComponent<PlayerHealth>(); // Reemplaza scriptVidaJugador
        }
    }

    //  MOVIMIENTO
    void HandleMovement(float distanceToPlayer)
    {
        if (!navAgent.isActiveAndEnabled) return;

        if (distanceToPlayer > touchRadius)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(player.position);
        }
        else
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    //  MERODEO
    void HandleWander()
    {
        if (!navAgent.isActiveAndEnabled) return;

        if (wanderState == WanderState.Waiting)
        {
            navAgent.isStopped = true;
            wanderWaitTimer -= Time.deltaTime;

            if (wanderWaitTimer <= 0f)
                TrySetWanderDestination();
        }
        else
        {
            CheckIfStuck();

            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                StartWaitTimer();
        }
    }

    void TrySetWanderDestination()
    {
        Vector3 randomPoint = spawnPosition + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = spawnPosition.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(hit.position);
            wanderState = WanderState.Moving;

            stuckTimer = 0f;
            lastRemainingDistance = float.MaxValue;
        }
        else
        {
            StartWaitTimer();
        }
    }

    void CheckIfStuck()
    {
        float remaining = navAgent.remainingDistance;

        if (Mathf.Abs(remaining - lastRemainingDistance) < stuckDistanceThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTimeLimit)
            {
                navAgent.ResetPath();
                StartWaitTimer();
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastRemainingDistance = remaining;
    }

    void StartWaitTimer()
    {
        wanderState     = WanderState.Waiting;
        wanderWaitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    //  DAÑO
    void HandleDamage(float distanceToPlayer)
    {
        if (distanceToPlayer <= touchRadius)
        {
            if (Time.time >= tiempoUltimoToque + cooldownEntreToques)
                AplicarEfectoSombra();
        }
    }

    void AplicarEfectoSombra()
    {
        tiempoUltimoToque = Time.time;

        if (playerHealth != null)
            playerHealth.TakeDamage(dañoPorToque, DamageType.Enemy, transform.position);
    }

    //  ANIMACIONES
    void HandleAnimations(float distanceToPlayer)
    {
        if (animator == null) return;

        int newState;

        if (distanceToPlayer <= touchRadius)
            newState = STATE_ATTACKING;
        else if (navAgent.isActiveAndEnabled && !navAgent.isStopped && navAgent.velocity.magnitude > 0.1f)
            newState = STATE_WALKING;
        else
            newState = STATE_IDLE;

        animator.SetInteger(StateParam, newState);
    }

    //  GIZMOS
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, touchRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? spawnPosition : transform.position, wanderRadius);
    }
}