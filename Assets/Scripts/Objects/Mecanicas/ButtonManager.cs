using UnityEngine;
using UnityEngine.Events;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private PressureButton[] buttons;

    public UnityEvent OnAllPressed;
    public UnityEvent OnAnyReleased;

    private void Start()
    {
        foreach (var button in buttons)
            button.OnStateChanged += CheckState;
    }

    private void OnDestroy()
    {
        foreach (var button in buttons)
            button.OnStateChanged -= CheckState;
    }

    private void CheckState()
    {
        foreach (var button in buttons)
        {
            if (!button.IsPressed)
            {
                OnAnyReleased?.Invoke();
                return;
            }
        }

        OnAllPressed?.Invoke();
    }
}