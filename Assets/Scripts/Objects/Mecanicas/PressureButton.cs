using UnityEngine;
using System;

public class PressureButton : MonoBehaviour
{
    [SerializeField] private Color unpressedColor = Color.red;
    [SerializeField] private Color pressedColor = Color.green;

    public bool IsPressed { get; private set; }
    public event Action OnStateChanged;

    private int boxCount = 0;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        SetColor(unpressedColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Box")) return;
        boxCount++;
        UpdateState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Box")) return;
        boxCount--;
        UpdateState();
    }

    private void UpdateState()
    {
        bool newState = boxCount > 0;
        if (newState == IsPressed) return;

        IsPressed = newState;
        SetColor(IsPressed ? pressedColor : unpressedColor);
        OnStateChanged?.Invoke();
    }

    private void SetColor(Color color)
    {
        propertyBlock.SetColor(BaseColor, color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
}