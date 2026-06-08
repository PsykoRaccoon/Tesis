using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { get; private set; }

    [Header("Jugador 1 - Muerte")]
    [SerializeField] private GameObject panelMuerteP1;
    [SerializeField] private TextMeshProUGUI textoConteoP1;

    [Header("Jugador 1 - Estado")]
    [SerializeField] private Image imagenVidaP1;
    [SerializeField] private Sprite iconoNormalP1;
    [SerializeField] private Sprite iconoContaminadoP1;
    [SerializeField] private Sprite iconoSombrificadoP1;

    [Header("Jugador 2 - Muerte")]
    [SerializeField] private GameObject panelMuerteP2;
    [SerializeField] private TextMeshProUGUI textoConteoP2;

    [Header("Jugador 2 - Estado")]
    [SerializeField] private Image imagenVidaP2;
    [SerializeField] private Sprite iconoNormalP2;
    [SerializeField] private Sprite iconoContaminadoP2;
    [SerializeField] private Sprite iconoSombrificadoP2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ---------------- MUERTE ---------------- //

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

    // ---------------- ESTADO DE VIDA ---------------- //

    public void ActualizarEstadoVida(int playerIndex, int currentHealth)
    {
        Image imagen = GetImagen(playerIndex);
        if (imagen == null) return;

        if (currentHealth >= 10)
            imagen.sprite = GetIconoNormal(playerIndex);
        else if (currentHealth >= 1)
            imagen.sprite = GetIconoContaminado(playerIndex);
        else
            imagen.sprite = GetIconoSombrificado(playerIndex);
    }

    // ---------------- GETTERS ---------------- //

    private GameObject GetPanel(int playerIndex) =>
        playerIndex == 1 ? panelMuerteP1 : panelMuerteP2;

    private TextMeshProUGUI GetTexto(int playerIndex) =>
        playerIndex == 1 ? textoConteoP1 : textoConteoP2;

    private Image GetImagen(int playerIndex) =>
        playerIndex == 1 ? imagenVidaP1 : imagenVidaP2;

    private Sprite GetIconoNormal(int playerIndex) =>
        playerIndex == 1 ? iconoNormalP1 : iconoNormalP2;

    private Sprite GetIconoContaminado(int playerIndex) =>
        playerIndex == 1 ? iconoContaminadoP1 : iconoContaminadoP2;

    private Sprite GetIconoSombrificado(int playerIndex) =>
        playerIndex == 1 ? iconoSombrificadoP1 : iconoSombrificadoP2;
}