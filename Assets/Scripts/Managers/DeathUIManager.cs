using UnityEngine;
using TMPro;

public class DeathUIManager : MonoBehaviour
{
    public static DeathUIManager Instance { get; private set; }

    [Header("Jugador 1")]
    [SerializeField] private GameObject panelMuerteP1;
    [SerializeField] private TextMeshProUGUI textoConteoP1;

    [Header("Jugador 2")]
    [SerializeField] private GameObject panelMuerteP2;
    [SerializeField] private TextMeshProUGUI textoConteoP2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowDeathPanel(int playerIndex)
    {
        GetPanel(playerIndex)?.SetActive(true);
    }

    public void HideDeathPanel(int playerIndex)
    {
        GetPanel(playerIndex)?.SetActive(false);
    }

    public void UpdateCountdownText(int playerIndex, float timeRemaining)
    {
        TextMeshProUGUI texto = GetTexto(playerIndex);
        if (texto != null)
            texto.text = "Reapareciendo en: " + Mathf.CeilToInt(timeRemaining);
    }

    private GameObject GetPanel(int playerIndex)
    {
        return playerIndex == 1 ? panelMuerteP1 : panelMuerteP2;
    }

    private TextMeshProUGUI GetTexto(int playerIndex)
    {
        return playerIndex == 1 ? textoConteoP1 : textoConteoP2;
    }
}