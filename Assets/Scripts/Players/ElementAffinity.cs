using UnityEngine;

public class ElementAffinity : MonoBehaviour
{
    [Tooltip("Elementos a los que este jugador es vulnerable")]
    [SerializeField] private Element[] vulnerableTo;

    public bool IsVulnerableTo(Element element)
    {
        foreach (Element e in vulnerableTo)
        {
            if (e == element) return true;
        }
        return false;
    }
}