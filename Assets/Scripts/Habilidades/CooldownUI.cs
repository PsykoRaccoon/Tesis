using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CooldownUI : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private Image iconImage;

    [Header("Colores")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color cooldownColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    [SerializeField] private Color lockedColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    private bool isLocked = false;

    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        SetAvailable();
    }

    public void SetOnCooldown(float duration)
    {
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = StartCoroutine(CooldownColorRoutine(duration));
    }

    public void SetUnavailable()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        iconImage.color = cooldownColor;
    }

    public void SetAvailable()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        iconImage.color = availableColor;
    }

    private IEnumerator CooldownColorRoutine(float duration)
    {
        iconImage.color = cooldownColor;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            iconImage.color = Color.Lerp(cooldownColor, availableColor, t);
            yield return null;
        }

        iconImage.color = availableColor;
        cooldownCoroutine = null;
    }

    public void SetLocked()
    {
        isLocked = true;
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        iconImage.color = lockedColor;
    }

    public void SetUnlocked()
    {
        isLocked = false;
        SetAvailable();
    }
}