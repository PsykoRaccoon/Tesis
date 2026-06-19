using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena
using UnityEngine.InputSystem; // Necesario para identificar a los jugadores
using TMPro; // Necesario para el texto del Canvas

public class Portal : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Escribe el nombre EXACTO de la escena a la que van a viajar")]
    public string nombreSiguienteEscena;

    [Header("Interfaz de Usuario (UI)")]
    public GameObject panelAviso;
    public TextMeshProUGUI textoAviso;

    // Variables internas para saber quién está pisando el portal
    private bool jugador1Adentro = false;
    private bool jugador2Adentro = false;

    void Start()
    {
        // Nos aseguramos de que el mensaje esté apagado al iniciar
        if (panelAviso != null)
            panelAviso.SetActive(false);
    }

    // Se activa cuando ALGUIEN entra al portal
    void OnTriggerEnter(Collider other)
    {
        // Verificamos si el que entró tiene la etiqueta de jugador
        if (other.CompareTag("Player"))
        {
            PlayerInput inputJugador = other.GetComponent<PlayerInput>();

            if (inputJugador != null)
            {
                // playerIndex 0 = Jugador 1 | playerIndex 1 = Jugador 2
                if (inputJugador.playerIndex == 0) jugador1Adentro = true;
                if (inputJugador.playerIndex == 1) jugador2Adentro = true;
            }

            RevisarEstadoPortal();
        }
    }

    // Se activa cuando ALGUIEN sale del portal (por si se arrepienten y se salen)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput inputJugador = other.GetComponent<PlayerInput>();

            if (inputJugador != null)
            {
                if (inputJugador.playerIndex == 0) jugador1Adentro = false;
                if (inputJugador.playerIndex == 1) jugador2Adentro = false;
            }

            RevisarEstadoPortal();
        }
    }

    void RevisarEstadoPortal()
    {
        // CASO 1: ¡Los dos están adentro! Viajamos.
        if (jugador1Adentro && jugador2Adentro)
        {
            if (panelAviso != null) panelAviso.SetActive(false);
            Debug.Log("¡Viajando a la siguiente escena!");
            SceneManager.LoadScene(nombreSiguienteEscena);
        }
        // CASO 2: Solo está el Jugador 1
        else if (jugador1Adentro && !jugador2Adentro)
        {
            MostrarMensaje("¡Falta el Jugador 2 en el portal!");
        }
        // CASO 3: Solo está el Jugador 2
        else if (!jugador1Adentro && jugador2Adentro)
        {
            MostrarMensaje("¡Falta el Jugador 1 en el portal!");
        }
        // CASO 4: El portal está vacío
        else
        {
            if (panelAviso != null) panelAviso.SetActive(false);
        }
    }

    void MostrarMensaje(string mensaje)
    {
        if (panelAviso != null)
        {
            panelAviso.SetActive(true);
            if (textoAviso != null)
            {
                textoAviso.text = mensaje;
            }
        }
    }
}