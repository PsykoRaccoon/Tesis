using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    public GameObject menuPausaP1;
    public GameObject menuPausaP2;

    [Header("Botones Iniciales (Para el Control)")]
    public GameObject primerBotonP1;
    public GameObject primerBotonP2;

    private bool estaPausado = false;
    private int jugadorQuePauso = 0;

    void Start()
    {
        if (menuPausaP1 != null) menuPausaP1.SetActive(false);
        if (menuPausaP2 != null) menuPausaP2.SetActive(false);
    }

    void Update()
    {
        // JUGADOR 1: Control 1 (Botón 7) o Tecla Escape
        if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Escape))
        {
            IntentarPausa(1);
        }

        // JUGADOR 2: Control 2 (Botón 7) o Tecla Return (Enter)
        if (Input.GetKeyDown(KeyCode.Joystick2Button3) || Input.GetKeyDown(KeyCode.Return))
        {
            IntentarPausa(2);
        }
    }

    public void IntentarPausa(int idJugador)
    {
        if (!estaPausado)
        {
            estaPausado = true;
            jugadorQuePauso = idJugador;
            Time.timeScale = 0f;

            // SEGURO: Verificamos que el EventSystem exista antes de usarlo
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                Debug.LogWarning("Falta un EventSystem en la escena. Créalo desde UI > Event System.");
            }

            if (idJugador == 1)
            {
                menuPausaP1.SetActive(true);
                if (EventSystem.current != null && primerBotonP1 != null)
                    EventSystem.current.SetSelectedGameObject(primerBotonP1);
            }
            if (idJugador == 2)
            {
                menuPausaP2.SetActive(true);
                if (EventSystem.current != null && primerBotonP2 != null)
                    EventSystem.current.SetSelectedGameObject(primerBotonP2);
            }
        }
        else if (estaPausado && jugadorQuePauso == idJugador)
        {
            ReanudarJuego();
        }
    }

    public void ReanudarJuego()
    {
        estaPausado = false;
        jugadorQuePauso = 0;
        Time.timeScale = 1f;

        menuPausaP1.SetActive(false);
        menuPausaP2.SetActive(false);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}