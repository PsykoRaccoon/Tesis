using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ActivatorOnHit : MonoBehaviour
{
    [Header("Tag que activa")]
    [SerializeField] private string reactionTag;

    [Header("Objeto a activar")]
    [SerializeField] private GameObject targetObject;

    private bool _activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (_activated) return;
        if (!other.CompareTag(reactionTag)) return;
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