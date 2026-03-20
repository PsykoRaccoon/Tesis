using UnityEngine;
using System;

public class ClonInstance : MonoBehaviour
{
    private Action onDestroyCallback;

    public void Init(Action callback)
    {
        onDestroyCallback = callback;
    }

    private void OnDestroy()
    {
        onDestroyCallback?.Invoke();
    }
}
