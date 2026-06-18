using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private GameObject[] elementPrefabs;
    [SerializeField] private Transform spawnPoint;

    private GameObject[] elementInstances;
    private int currentIndex = 0;

    private void Start()
    {
        elementInstances = new GameObject[elementPrefabs.Length];

        for (int i = 0; i < elementPrefabs.Length; i++)
        {
            elementInstances[i] = Instantiate(elementPrefabs[i], spawnPoint.position, spawnPoint.rotation, spawnPoint);
            elementInstances[i].SetActive(i == 0);
        }
    }

    public void CycleElement()
    {
        elementInstances[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % elementInstances.Length;
        elementInstances[currentIndex].SetActive(true);
    }
}