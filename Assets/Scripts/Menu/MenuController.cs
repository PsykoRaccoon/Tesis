using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("Levels To Load")]
    public string newGameLevel;
    private string levelToLoad;

    [Header("Contenedores Principales")]
    [SerializeField] private GameObject menuPrincipalContainer;

    [Header("Paneles de Diálogo")]
    [SerializeField] private GameObject newGameDialogPanel;
    [SerializeField] private GameObject loadGameDialogPanel;
    [SerializeField] private GameObject noSaveGameDialog;

    [Header("Botones Menú Principal")]
    public GameObject botonNJ;
    public GameObject botonCJ;

    [Header("Botones Primeros en Diálogos")]
    public GameObject botonSiDialogoNJ;
    public GameObject botonSiDialogoCJ;
    public GameObject botonOkDialogoError;

    void Start()
    {
        if (EventSystem.current != null && botonNJ != null)
        {
            EventSystem.current.SetSelectedGameObject(botonNJ);
        }
    }

    void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            if (newGameDialogPanel != null && newGameDialogPanel.activeInHierarchy)
                CerrarDialogoNuevoJuego();
            else if (loadGameDialogPanel != null && loadGameDialogPanel.activeInHierarchy)
                CerrarDialogoCargarJuego();
            else if (noSaveGameDialog != null && noSaveGameDialog.activeInHierarchy)
                CerrarDialogoNoSave();
        }
    }

    // --- NUEVO JUEGO ---
    public void AbrirDialogoNuevoJuego()
    {
        // 1. ENCENDER EL NUEVO PRIMERO
        newGameDialogPanel.SetActive(true);

        // 2. ROBAR EL FOCO (Ambos menús existen un milisegundo, así que no hay error)
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonSiDialogoNJ);

        // 3. APAGAR EL VIEJO AL FINAL
        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(false);
    }

    public void CerrarDialogoNuevoJuego()
    {
        // 1. ENCENDER EL MENÚ PRINCIPAL PRIMERO
        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(true);

        // 2. DEVOLVER EL FOCO
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonNJ);

        // 3. APAGAR EL DIÁLOGO AL FINAL
        newGameDialogPanel.SetActive(false);
    }

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    // --- CARGAR JUEGO ---
    public void AbrirDialogoCargarJuego()
    {
        loadGameDialogPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonSiDialogoCJ);

        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(false);
    }

    public void CerrarDialogoCargarJuego()
    {
        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonCJ);

        loadGameDialogPanel.SetActive(false);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            // Ojo aquí también con el orden
            noSaveGameDialog.SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botonOkDialogoError);

            loadGameDialogPanel.SetActive(false);
        }
    }

    public void CerrarDialogoNoSave()
    {
        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonCJ);

        noSaveGameDialog.SetActive(false);
    }
}