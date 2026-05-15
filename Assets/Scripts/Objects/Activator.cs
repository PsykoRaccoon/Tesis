using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Activator : MonoBehaviour
{
    [Header("Tag que activa")]
    [SerializeField] private string reactionTag;

    [Header("Objeto a activar")]
    [SerializeField] private GameObject targetObject;

    private bool _activated = false;

    // Bola de fuego (y cualquier otro trigger normal)
    void OnTriggerEnter(Collider other)
    {
        if (_activated) return;
        if (!other.CompareTag(reactionTag)) return;
        Activate();
    }

    // Rayo láser → llamado via SendMessage desde LaserAbility
    void OnLaserEnter(GameObject source)
    {
        if (_activated) return;
        if (!source.CompareTag(reactionTag)) return;
        Activate();
    }

    private void Activate()
    {
        _activated = true;

        if (targetObject != null)
            targetObject.SetActive(true);
        else
            Debug.LogWarning("ActivatorOnHit: No hay objeto asignado en Target Object!");
    }
}