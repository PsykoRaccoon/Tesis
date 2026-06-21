using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    [Header("Spawn Points Iniciales")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Offset al spawnear junto al compañero")]
    [SerializeField] private float companionSpawnOffset;

    private Transform player1Transform;
    private Transform player2Transform;
    private PlayerHealth player1Health;
    private PlayerHealth player2Health;

    private int zonaActual = 0;
    private Vector3 zonaSpawn1;
    private Vector3 zonaSpawn2;
    private bool zonaPermiteCompanero = false;

    private SplitScreenManager splitScreenManager;

    void Start()
    {
        splitScreenManager = FindObjectOfType<SplitScreenManager>();
        zonaSpawn1 = player1Spawn.position;
        zonaSpawn2 = player2Spawn.position;
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        foreach (var p in FindObjectsOfType<PlayerInput>())
            Destroy(p.gameObject);

        var p1Input = PlayerInput.Instantiate(
            player1Prefab,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all.Count > 0 ? Gamepad.all[0] : null
        );
        player1Transform = p1Input.transform;
        player1Health = p1Input.GetComponent<PlayerHealth>();
        StartCoroutine(SafeSetPosition(player1Transform, player1Spawn.position));

        if (Gamepad.all.Count > 1)
        {
            var p2Input = PlayerInput.Instantiate(
                player2Prefab,
                controlScheme: "Gamepad",
                pairWithDevice: Gamepad.all[1]
            );
            player2Transform = p2Input.transform;
            player2Health = p2Input.GetComponent<PlayerHealth>();
            StartCoroutine(SafeSetPosition(player2Transform, player2Spawn.position));
        }

        if (splitScreenManager != null)
            splitScreenManager.AssignPlayers(player1Transform, player2Transform);
    }

    private IEnumerator SafeSetPosition(Transform target, Vector3 position)
    {
        var cc = target.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        yield return null; // espera un frame

        target.position = position;
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;
    }

    public void TryUpdateZone(int newIndex, Vector3 spawn1, Vector3 spawn2, bool permiteCompanero)
    {
        if (newIndex <= zonaActual) return;

        zonaActual = newIndex;
        zonaSpawn1 = spawn1;
        zonaSpawn2 = spawn2;
        zonaPermiteCompanero = permiteCompanero;

        Debug.Log($"Zona actualizada a {zonaActual} | Spawn junto al compañero: {zonaPermiteCompanero}");
    }

    public Vector3 GetRespawnPosition(int playerIndex)
    {
        if (zonaPermiteCompanero)
        {
            bool p1Vivo = player1Health != null && !player1Health.IsDead;
            bool p2Vivo = player2Health != null && !player2Health.IsDead;

            if (playerIndex == 0 && p2Vivo)
                return player2Transform.position + Vector3.right * companionSpawnOffset;

            if (playerIndex == 1 && p1Vivo)
                return player1Transform.position - Vector3.right * companionSpawnOffset;
        }

        return playerIndex == 0 ? zonaSpawn1 : zonaSpawn2;
    }
}