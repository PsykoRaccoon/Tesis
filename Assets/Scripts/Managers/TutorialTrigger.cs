using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private VideoClip tutorialClip;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject controlsUI;

    [Header("Config")]
    [SerializeField] private int countdownSeconds;

    [Header("Eventos")]
    [SerializeField] private UnityEvent onTutorialComplete;

    private bool _hasBeenTriggered = false;

    private void Start()
    {
        tutorialCanvas.SetActive(false);
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasBeenTriggered = true;
        StartTutorial();
    }

    private void StartTutorial()
    {
        Time.timeScale = 0f;
        tutorialCanvas.SetActive(true);
        pauseMenu.SetActive(false);
        controlsUI.SetActive(true);
        countdownText.gameObject.SetActive(false);
        videoPlayer.clip = tutorialClip;
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log($"{gameObject.name} recibió OnVideoFinished");
        StartCoroutine(CountdownAndClose());
    }

    private IEnumerator CountdownAndClose()
    {
        countdownText.gameObject.SetActive(true);
        videoPlayer.loopPointReached -= OnVideoFinished;

        for (int i = countdownSeconds; i > 0; i--)
        {
            countdownText.text = $"Continuando en {i}...";
            yield return new WaitForSecondsRealtime(1f);
        }

        tutorialCanvas.SetActive(false);
        pauseMenu.SetActive(true);
        controlsUI.SetActive(false);
        Time.timeScale = 1f;
        onTutorialComplete?.Invoke();
        Destroy(gameObject);
    }
}