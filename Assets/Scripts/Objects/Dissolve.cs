using System.Collections;
using UnityEngine;

public enum ReactElement { Fire, Water }

[RequireComponent(typeof(Renderer))]
public class Dissolve : MonoBehaviour
{
    [Header("Elemento que activa la reaccion")]
    [SerializeField] private ReactElement reactTo;

    [Header("Shader")]
    [SerializeField] private string shaderProperty = "_BurntAmount";
    [SerializeField] private float burnFrom;
    [SerializeField] private float burnTo;
    [SerializeField] private float duration;

    [Header("Collider sólido (bloquea al jugador)")]
    [SerializeField] private Collider solidCollider;

    [Header("VFX hijos a desactivar (solo Water)")]
    [SerializeField] private GameObject[] vfxToDisable;

    [Header("Al finalizar")]
    [SerializeField] private bool destroyOnFinish = false;

    private static readonly int ShaderProp = Shader.PropertyToID("_BurntAmount");

    private Material[] _mats;
    private Collider   _triggerCollider;
    private bool       _reacting = false;

    void Awake()
    {
        _triggerCollider = GetComponent<Collider>();

        _mats = GetComponent<Renderer>().materials;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_reacting) return;

        bool isFire  = reactTo == ReactElement.Fire  && other.CompareTag("Fire");
        bool isWater = reactTo == ReactElement.Water && other.CompareTag("Water");

        if (!isFire && !isWater) return;

        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        _reacting = true;

        float from = burnFrom;
        float to = burnTo;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            SetShader(value);
            yield return null;
        }

        SetShader(to);

        if (reactTo == ReactElement.Fire && solidCollider != null)
        {
            solidCollider.enabled = false;
            _triggerCollider.enabled = false; 
        }

        // Solo Water: apaga los VFX hijos
        if (reactTo == ReactElement.Water && vfxToDisable != null)
        {
            foreach (var vfx in vfxToDisable)
            {
                if (vfx != null) vfx.SetActive(false);
                
            }
        }

        if (reactTo == ReactElement.Water)
        {
            _triggerCollider.enabled = false;
        }

        if (destroyOnFinish)
        {
            Destroy(gameObject);
        }
    }

    private void SetShader(float value)
    {
        foreach (var mat in _mats)
        {
            if (mat.HasProperty(shaderProperty))
            {
                mat.SetFloat(shaderProperty, value);
            }
        }
    }
}