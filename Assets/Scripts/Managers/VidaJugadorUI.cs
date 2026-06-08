using UnityEngine;
using UnityEngine.UI;

public class VidaJugadorUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image imagenVidaUI;

    [Header("Sprites de Estado")]
    public Sprite iconoNormal;
    public Sprite iconoContaminado;
    public Sprite iconoSombrificado;

    public void ActualizarEstado(int currentHealth)
    {
        if (imagenVidaUI == null) return;

        if (currentHealth >= 10)
            imagenVidaUI.sprite = iconoNormal;
        else if (currentHealth >= 1)
            imagenVidaUI.sprite = iconoContaminado;
        else
            imagenVidaUI.sprite = iconoSombrificado;
    }
}