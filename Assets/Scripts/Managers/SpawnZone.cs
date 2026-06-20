using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [Header("Zona")]
    [SerializeField] private int zoneIndex;
    [SerializeField] private bool spawnJuntoAlCompanero = false;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private SpawnManager spawnManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (spawnManager == null)
            spawnManager = FindObjectOfType<SpawnManager>();

        spawnManager.TryUpdateZone(zoneIndex, spawnPoint1.position, spawnPoint2.position, spawnJuntoAlCompanero);
    }
}