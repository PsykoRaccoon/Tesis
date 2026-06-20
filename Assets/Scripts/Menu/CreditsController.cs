using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    [Header("Escena destino")]
    [SerializeField] private string mainMenuScene;

    [Header("Boton inicial")]
    [SerializeField] private GameObject botonVolver;

    void Start()
    {
        StartCoroutine(SelectInitialButton());
    }

    private IEnumerator SelectInitialButton()
    {
        yield return null;
        if (EventSystem.current != null && botonVolver != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botonVolver);
        }
    }

    void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}