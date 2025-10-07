using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 3f;

    private Transform player;
    private EnemyHealth health;

    private void Start()
    {
        // Buscar al jugador (asumiendo que tiene tag "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        health = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (player == null || health.IsDead) return;

        // Movimiento simple hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotación hacia el jugador
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.2f);
        }
    }
}
