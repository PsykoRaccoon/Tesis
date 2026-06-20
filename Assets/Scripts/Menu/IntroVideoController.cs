using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class IntroVideoController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Escena")]
    [SerializeField] private string gameplayScene;

    [Header("Music")]
    [SerializeField] private MusicManager musicManager;

    [Header("Fade Visual")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("Boton Continuar")]
    [SerializeField] private CanvasGroup skipButtonCanvasGroup;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private float tiempoParaMostrarBoton;
    [SerializeField] private float duracionFadeIn;

    private bool escenaCargada = false;

    void Start()
    {
        skipButtonCanvasGroup.alpha = 0f;
        skipButtonCanvasGroup.interactable = false;
        skipButtonCanvasGroup.blocksRaycasts = false;
        skipButton.SetActive(false);

        videoPlayer.loopPointReached += OnVideoTerminado;

        StartCoroutine(MostrarBotonDespuesDeEspera());
    }

    private IEnumerator MostrarBotonDespuesDeEspera()
    {
        yield return new WaitForSeconds(tiempoParaMostrarBoton);
        yield return StartCoroutine(FadeInBoton());
    }

    private IEnumerator FadeInBoton()
    {
        EventSystem.current.SetSelectedGameObject(skipButton);
        skipButton.SetActive(true);

        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(skipButton);

        float tiempo = 0f;
        while (tiempo < duracionFadeIn)
        {
            tiempo += Time.deltaTime;
            skipButtonCanvasGroup.alpha = Mathf.Clamp01(tiempo / duracionFadeIn);
            yield return null;
        }

        skipButtonCanvasGroup.alpha = 1f;
        skipButtonCanvasGroup.interactable = true;
        skipButtonCanvasGroup.blocksRaycasts = true;
    }

    private void OnVideoTerminado(VideoPlayer vp)
    {
        CargarGameplay();
    }

    public void OnSkipPressed()
    {
        CargarGameplay();
    }

    private void CargarGameplay()
    {
        if (escenaCargada) return;
        escenaCargada = true;

        if (musicManager != null)
            musicManager.FadeOutAndLoad(gameplayScene, fadePanel);
        else
            SceneManager.LoadScene(gameplayScene);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoTerminado;
    }
}