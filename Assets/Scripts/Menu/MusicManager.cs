using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicClip;

    [Header("Fade Out")]
    [SerializeField] private float fadeOutDuration = 1.5f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void FadeOutAndLoad(string sceneName, CanvasGroup fadePanel = null)
    {
        StartCoroutine(FadeOutRoutine(sceneName, fadePanel));
    }

    private IEnumerator FadeOutRoutine(string sceneName, CanvasGroup fadePanel)
    {
        float volumenInicial = audioSource.volume;
        float tiempo = 0f;

        while (tiempo < fadeOutDuration)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / fadeOutDuration;

            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, t);

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        if (fadePanel != null)
            fadePanel.alpha = 1f;

        SceneManager.LoadScene(sceneName);
    }
}