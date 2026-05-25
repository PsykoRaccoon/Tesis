using UnityEngine;
using System.Collections.Generic;

public class AbilityActivator : MonoBehaviour
{
    private int playersActivated = 0;
    private const int totalPlayers = 2;
    private HashSet<GameObject> activatedPlayers = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (activatedPlayers.Contains(other.gameObject)) return;

        var manager = other.GetComponent<AbilityManager>();
        if (manager != null) manager.UnlockAndKeepCurrent();

        activatedPlayers.Add(other.gameObject);
        playersActivated++;

        if (playersActivated >= totalPlayers)
        {
            gameObject.SetActive(false);
            Debug.Log("Ya pasaron ambos jugadores, ya tienen sus dos habilidades desbloqueadas");
        }
    }
}