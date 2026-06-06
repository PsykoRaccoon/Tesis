using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : Health
{
    [Header("Death Settings")]
    [SerializeField] private float deathDelay;

    private Animator animator;
    private NavMeshAgent navAgent;

    private static readonly int StateParam = Animator.StringToHash("State");

    private const int STATE_DEAD = 3;

    protected override void Awake()
    {
        base.Awake();
        animator  = GetComponent<Animator>();
        navAgent  = GetComponent<NavMeshAgent>();
    }

    protected override void Die(DamageType type)
    {
        if (isDead) return;
        isDead = true;

        if (navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }

        if (animator != null)
            animator.SetInteger(StateParam, STATE_DEAD);

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}