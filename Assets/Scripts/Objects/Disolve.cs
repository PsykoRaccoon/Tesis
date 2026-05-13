using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class Disolve : MonoBehaviour
{
    [Header("Tag")]
    [SerializeField] private string reactionTag;

    [Header("Shader")]
    [SerializeField] private string shaderProperty = "_BurntAmount";
    [SerializeField] private float burntFrom;
    [SerializeField] private float burntTo;
    [SerializeField] private float duration;

    [Header("Al finalizar")]
    [SerializeField] private bool desactivarCollider = true;
    [SerializeField] private bool destruirObjeto     = false;

    private Material[] _mats;
    private Collider   _col;
    private bool _reacting = false;
    public GameObject wall;

    void Awake()
    {
        _col = GetComponent<Collider>();
        wall.SetActive(true);

        _mats = GetComponent<Renderer>().materials;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_reacting) return;
        if (!other.CompareTag(reactionTag)) return;
        StartCoroutine(BurntRoutine());
    }

    private IEnumerator BurntRoutine()
    {
        _reacting = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t     = Mathf.Clamp01(elapsed / duration);
            float valor = Mathf.Lerp(burntFrom, burntTo, t);

            foreach (var mat in _mats)
            {
                if (mat.HasProperty(shaderProperty))
                    mat.SetFloat(shaderProperty, valor);
            }

            yield return null;
        }

        foreach (var mat in _mats)
        {
            if (mat.HasProperty(shaderProperty))
                mat.SetFloat(shaderProperty, burntTo);
        }

        if (desactivarCollider)
            _col.enabled = false;

        if (destruirObjeto)
        {
            Destroy(gameObject);
            wall.SetActive(false);
        }
    }
}