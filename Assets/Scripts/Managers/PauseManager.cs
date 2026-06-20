using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    [SerializeField] private GameObject menuPausaP1;
    [SerializeField] private GameObject menuPausaP2;
    [SerializeField] private GameObject panelControles;
    [SerializeField] private GameObject atrasBtn;

    [Header("Botones Iniciales (Para el Control)")]
    [SerializeField] private GameObject primerBotonP1;
    [SerializeField] private GameObject primerBotonP2;

    [Header("Input")]
    [SerializeField] private UIInputActions inputActions;

    private bool estaPausado = false;
    private int jugadorQuePauso = 1;

    private void Awake()
    {
        inputActions = new UIInputActions();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Pause.performed += OnPausePerformed;
        inputActions.UI.Pause.performed += OnPausePerformed;
        inputActions.Gameplay.Enable();
        inputActions.UI.Disable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Pause.performed -= OnPausePerformed;
        inputActions.UI.Pause.performed -= OnPausePerformed;
        inputActions.Disable();
    }

    private void Start()
    {
        if (menuPausaP1 != null) menuPausaP1.SetActive(false);
        if (menuPausaP2 != null) menuPausaP2.SetActive(false);
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        if (!estaPausado)
            Pausar(DetectarJugador(ctx));
        else
            Reanudar();
    }

    private int DetectarJugador(InputAction.CallbackContext ctx)
    {
        var gamepad = ctx.control.device as Gamepad;

        if (gamepad == null) return 1;

        if (Gamepad.all.Count > 0 && gamepad == Gamepad.all[0]) return 1;
        if (Gamepad.all.Count > 1 && gamepad == Gamepad.all[1]) return 2;

        return 1;
    }

    private void Pausar(int jugador)
    {
        jugadorQuePauso = jugador;
        estaPausado = true;
        Time.timeScale = 0f;

        inputActions.Gameplay.Disable();
        inputActions.UI.Enable();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (jugador == 1 && menuPausaP1 != null)
        {
            menuPausaP1.SetActive(true);
            if (primerBotonP1 != null)
                EventSystem.current.SetSelectedGameObject(primerBotonP1);
        }
        else if (jugador == 2 && menuPausaP2 != null)
        {
            menuPausaP2.SetActive(true);
            if (primerBotonP2 != null)
            {
                EventSystem.current.SetSelectedGameObject(primerBotonP2);
            }
        }
    }

    public void Reanudar()
    {
        estaPausado = false;
        Time.timeScale = 1f;

        inputActions.UI.Disable();
        inputActions.Gameplay.Enable();

        if (menuPausaP1 != null) menuPausaP1.SetActive(false);
        if (menuPausaP2 != null) menuPausaP2.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void IrAlMenuPrincipal(string nombreEscena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }

    public void Controles()
    {
        if (panelControles != null)
        {
            panelControles.SetActive(true);
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(atrasBtn);
            }
        }
    }

    public void VolverAlMenuPausa()
    {
        if (panelControles != null)
        {
            panelControles.SetActive(false);
            if (EventSystem.current != null)
            {
                var botonDestino = jugadorQuePauso == 1 ? primerBotonP1 : primerBotonP2;
                EventSystem.current.SetSelectedGameObject(botonDestino);
            }
        }
    }
}