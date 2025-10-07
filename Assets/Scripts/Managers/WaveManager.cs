using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// -------------------- DATA STRUCTURES --------------------
[Serializable]
public class EnemySpawnData
{
    public int Enemy;   // ID del enemigo
    public float Time;  // Momento de aparición en la oleada
}

[Serializable]
public class WaveData
{
    public int Wave;
    public List<EnemySpawnData> Enemies;
}

[Serializable]
public class WavesContainer
{
    public List<WaveData> Waves;
}

// -------------------- ENEMY TRACKER --------------------
public class EnemyTracker : MonoBehaviour
{
    private static int enemiesAlive = 0;

    public static event Action OnAllEnemiesDefeated;

    private void OnEnable()
    {
        enemiesAlive++;
    }

    private void OnDestroy()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
    }
}

// -------------------- WAVE MANAGER --------------------
public class WaveManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private EnemyFactory enemyFactory;

    [Header("JSON Settings")]
    [SerializeField] private string wavesUrl = "https://kev-games-development.net/Services/WavesTest.json";

    private WavesContainer wavesData;
    private int currentWaveIndex = 0;

    private void Start()
    {
        StartCoroutine(LoadWavesFromJson());
    }

    private IEnumerator LoadWavesFromJson()
    {
        UnityWebRequest request = UnityWebRequest.Get(wavesUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error cargando JSON: {request.error}");
            yield break;
        }

        string json = request.downloadHandler.text;
        wavesData = JsonUtility.FromJson<WavesContainer>(json);

        Debug.Log("JSON cargado correctamente. Iniciando oleadas...");
        StartCoroutine(HandleWaves());
    }

    private IEnumerator HandleWaves()
    {
        while (currentWaveIndex < wavesData.Waves.Count)
        {
            WaveData wave = wavesData.Waves[currentWaveIndex];
            Debug.Log($"Iniciando oleada {wave.Wave}");

            yield return StartCoroutine(SpawnWave(wave));

            // Esperar a que mueran todos los enemigos
            bool waveFinished = false;
            EnemyTracker.OnAllEnemiesDefeated += () => waveFinished = true;

            yield return new WaitUntil(() => waveFinished);

            Debug.Log($"Oleada {wave.Wave} completada");
            currentWaveIndex++;
        }

        Debug.Log("¡Todas las oleadas completadas!");
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (EnemySpawnData enemyData in wave.Enemies)
        {
            yield return new WaitForSeconds(enemyData.Time);

            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject enemy = enemyFactory.CreateEnemy(enemyData.Enemy, spawnPoint.position);

            if (enemy != null && enemy.GetComponent<EnemyTracker>() == null)
            {
                enemy.AddComponent<EnemyTracker>();
            }
        }
    }
}
