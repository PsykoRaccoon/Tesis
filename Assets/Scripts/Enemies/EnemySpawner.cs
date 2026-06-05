using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    [Tooltip("Arrastra aquí tu Prefab de la Sombra Básica desde la carpeta del proyecto.")]
    public GameObject enemigoPrefab;

    [Tooltip("Lista de puntos vacíos en el mapa donde pueden aparecer los enemigos.")]
    public Transform[] spawnPoints;

    [Header("Ajustes de Tiempo y Cantidad")]
    [Tooltip("Segundos que tardará en aparecer un nuevo enemigo.")]
    public float tiempoEntreSpawns = 5f;

    [Tooltip("Límite máximo de enemigos vivos al mismo tiempo para no saturar el juego.")]
    public int maxEnemigosVivos = 5;

    // Llevamos la cuenta interna
    private int enemigosActuales = 0;

    void Start()
    {
        // Verificamos que no falte nada antes de empezar
        if (enemigoPrefab == null)
        {
            Debug.LogError("🚨 Falta asignar el Prefab del enemigo en el Spawner.");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("🚨 No has asignado ningún SpawnPoint al Spawner.");
            return;
        }

        // Iniciamos la rutina de aparición
        StartCoroutine(RutinaDeSpawneo());
    }

    IEnumerator RutinaDeSpawneo()
    {
        // Un ciclo infinito que se pausa con el "yield"
        while (true)
        {
            // Pausamos la rutina por los segundos que elegiste
            yield return new WaitForSeconds(tiempoEntreSpawns);

            // Si aún no llegamos al límite, creamos un enemigo
            if (enemigosActuales < maxEnemigosVivos)
            {
                SpawnearEnemigo();
            }
        }
    }

    void SpawnearEnemigo()
    {
        // 1. Elegimos un punto de aparición al azar de tu lista
        int indiceAleatorio = Random.Range(0, spawnPoints.Length);
        Transform puntoElegido = spawnPoints[indiceAleatorio];

        // 2. Creamos (Instanciamos) el Prefab exactamente en ese punto
        Instantiate(enemigoPrefab, puntoElegido.position, puntoElegido.rotation);

        // 3. Aumentamos el contador
        enemigosActuales++;
    }

    // --- Función pública por si la Sombra muere y quieres liberar espacio ---
    public void EnemigoEliminado()
    {
        enemigosActuales--;
        // Nos aseguramos de que no baje de cero por algún error
        if (enemigosActuales < 0) enemigosActuales = 0;
    }
}