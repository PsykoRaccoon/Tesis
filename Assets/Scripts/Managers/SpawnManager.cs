using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    [Header("Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    private SplitScreenManager splitScreenManager;

    void Start()
    {
        splitScreenManager = FindObjectOfType<SplitScreenManager>();
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        // limpiar jugadores existentes
        foreach (var p in FindObjectsOfType<PlayerInput>())
            Destroy(p.gameObject);

        // jugador 1
        var player1 = PlayerInput.Instantiate(
            player1Prefab,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all.Count > 0 ? Gamepad.all[0] : null
        );
        player1.transform.position = player1Spawn.position;

        // jugador 2
        GameObject player2 = null;
        if (Gamepad.all.Count > 1)
        {
            player2 = PlayerInput.Instantiate(
                player2Prefab,
                controlScheme: "Gamepad",
                pairWithDevice: Gamepad.all[1]
            ).gameObject;
            player2.transform.position = player2Spawn.position;
        }
        
        if (splitScreenManager != null)
        {
            splitScreenManager.AssignPlayers(player1.transform, player2 != null ? player2.transform : null);
        }
    }
}
