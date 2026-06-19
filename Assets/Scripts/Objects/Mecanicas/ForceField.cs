using UnityEngine;

[RequireComponent(typeof(DamageDealer))]
public class ForceField : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float liftForce = 10f;

    private DamageDealer _damageDealer;
    private Element _padElement;

    private void Awake()
    {
        _damageDealer = GetComponent<DamageDealer>();
        // Lee el elemento directamente del DamageDealer via reflection
        // para no romper tu arquitectura actual
        var field = typeof(DamageDealer)
            .GetField("damageElement", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);

        if (field != null)
            _padElement = (Element)field.GetValue(_damageDealer);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        ElementAffinity affinity = other.GetComponentInParent<ElementAffinity>();
        if (affinity == null) return;

        // Si es vulnerable = incompatible, el DamageDealer ya se encarga
        if (affinity.IsVulnerableTo(_padElement)) return;

        player.SetLift(liftForce);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        player.ClearLift();
    }
}