using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.InputSystem; 
using TMPro; 

public class Portal : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Escribe el nombre EXACTO de la escena a la que van a viajar")]
    public string nombreSiguienteEscena;

    [Header("Interfaz de Usuario (UI)")]
    public GameObject panelAviso;
    public TextMeshProUGUI textoAviso;

    private bool jugador1Adentro = false;
    private bool jugador2Adentro = false;

    void Start()
    {
        if (panelAviso != null)
        {
            panelAviso.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
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
        if (jugador1Adentro && jugador2Adentro)
        {
            if (panelAviso != null) panelAviso.SetActive(false);
            Debug.Log("¡Viajando a la siguiente escena!");
            SceneManager.LoadScene(nombreSiguienteEscena);
        }
        else if (jugador1Adentro && !jugador2Adentro)
        {
            MostrarMensaje("¡Falta el Jugador 2 en el portal!");
        }
        else if (!jugador1Adentro && jugador2Adentro)
        {
            MostrarMensaje("¡Falta el Jugador 1 en el portal!");
        }
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