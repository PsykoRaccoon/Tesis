using UnityEngine;
using UnityEngine.UI;

public class VidaJugadorUI : MonoBehaviour
{
    [Header("Configuración de UI")]
    [Tooltip("Escribe aquí el nombre EXACTO del objeto en tu Canvas (ej. 'VidaPlayer1' o 'VidaPlayer2')")]
    public string nombreDelObjetoUI = "VidaPlayer1";

    // Ya no es public, porque el script la va a encontrar automáticamente
    private Image imagenVidaUI;

    [Header("Sprites de Estado")]
    public Sprite iconoNormal;
    public Sprite iconoContaminado;
    public Sprite iconoSombrificado;

    private int toquesRecibidos = 0;

    void Start()
    {
        // 1. AUTODESCUBRIMIENTO: El jugador busca el objeto de UI por su nombre en la escena
        GameObject objetoUIEnCanvas = GameObject.Find(nombreDelObjetoUI);

        // 2. Si lo encuentra, extrae el componente 'Image' y se conecta
        if (objetoUIEnCanvas != null)
        {
            imagenVidaUI = objetoUIEnCanvas.GetComponent<Image>();

            // 3. Reiniciamos la máscara para que esté limpia al empezar
            if (imagenVidaUI != null && iconoNormal != null)
            {
                imagenVidaUI.sprite = iconoNormal;
            }
        }
        else
        {
            Debug.LogError("🚨 ERROR: El jugador no pudo encontrar un objeto en el Canvas que se llame: " + nombreDelObjetoUI);
        }
    }

    // --- LÓGICA DE DAÑO ---
    public void RecibirToqueSombra()
    {
        toquesRecibidos++;

        if (toquesRecibidos == 1)
        {
            if (imagenVidaUI != null && iconoContaminado != null)
                imagenVidaUI.sprite = iconoContaminado;

            Debug.Log("⚠️ ESTADO: Contaminado. (Bloquear cambio de elemento)");
        }
        else if (toquesRecibidos >= 2)
        {
            if (imagenVidaUI != null && iconoSombrificado != null)
                imagenVidaUI.sprite = iconoSombrificado;

            Debug.Log("☠️ ESTADO: Sombrificado. (Muerte del jugador)");
        }
    }
}