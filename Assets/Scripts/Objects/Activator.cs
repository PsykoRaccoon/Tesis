using UnityEngine;

public class ActivatorOnHit : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string fireTag;
    [SerializeField] private string waterTag;

    [Header("Objeto a controlar")]
    [SerializeField] private GameObject targetObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(fireTag))       Activate();
        else if (other.CompareTag(waterTag)) Deactivate();
    }

    void OnLaserEnter(GameObject source)
    {
        if (source.CompareTag(fireTag))       Activate();
        else if (source.CompareTag(waterTag)) Deactivate();
    }

    private void Activate()
    {
        if (targetObject == null) { LogWarning(); return; }
        if (targetObject.activeSelf) return;
        targetObject.SetActive(true);
    }

    private void Deactivate()
    {
        if (targetObject == null) { LogWarning(); return; }
        if (!targetObject.activeSelf) return;
        targetObject.SetActive(false);
    }

    private void LogWarning() =>
        Debug.LogWarning($"ActivatorOnHit ({gameObject.name}): No hay objeto asignado en Target Object!");
}