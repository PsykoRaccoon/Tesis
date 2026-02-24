using UnityEngine;

public class WallCutoutController : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        Shader.SetGlobalVector("_PlayerPosition", transform.position);
    }
}