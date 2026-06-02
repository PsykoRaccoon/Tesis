using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

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
        StartCoroutine(SelectInitialButton());
    }

    private IEnumerator SelectInitialButton()
    {
        yield return null; // espera un frame
        if (EventSystem.current != null && botonNJ != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
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
        newGameDialogPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonSiDialogoNJ);

        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(false);
    }

    public void CerrarDialogoNuevoJuego()
    {
        if (menuPrincipalContainer != null) menuPrincipalContainer.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botonNJ);

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