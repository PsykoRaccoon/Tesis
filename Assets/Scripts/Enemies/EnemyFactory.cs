using UnityEngine;
using System.Collections.Generic;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;

    public GameObject CreateEnemy(int enemyId, Vector3 position)
    {
        if (enemyId <= 0 || enemyId > enemyPrefabs.Count)
        {
            Debug.LogError($"Enemy ID {enemyId} fuera de rango");
            return null;
        }

        return Instantiate(enemyPrefabs[enemyId - 1], position, Quaternion.identity);
    }
}
