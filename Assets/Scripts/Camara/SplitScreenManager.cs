using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Jugadores")]
    public Transform player1;
    public Transform player2;

    [Header("Cámaras")]
    public Camera mainCamera;
    public Camera player1Camera;
    public Camera player2Camera;

    [Header("Parámetros")]
    public float splitDistance = 20f; // distancia para activar split screen
    public float cameraHeight = 30f;
    public float cameraAngle = 45f;

    [Header("UI")]
    public GameObject dividerLine; // panel del Canvas que será la línea divisoria

    void Update()
    {
        float distance = Vector3.Distance(player1.position, player2.position);

        if (distance < splitDistance)
        {
            // --- Cámara compartida ---
            mainCamera.gameObject.SetActive(true);
            player1Camera.gameObject.SetActive(false);
            player2Camera.gameObject.SetActive(false);

            // centrar cámara entre los 2
            Vector3 midPoint = (player1.position + player2.position) / 2f;
            mainCamera.transform.position = midPoint + new Vector3(0, cameraHeight, -cameraHeight);
            mainCamera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);

            // ocultar línea divisoria
            if (dividerLine != null)
                dividerLine.SetActive(false);
        }
        else
        {
            // --- Split Screen ---
            mainCamera.gameObject.SetActive(false);
            player1Camera.gameObject.SetActive(true);
            player2Camera.gameObject.SetActive(true);

            // Cámara jugador 1
            player1Camera.transform.position = player1.position + new Vector3(0, cameraHeight, -cameraHeight);
            player1Camera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);

            // Cámara jugador 2
            player2Camera.transform.position = player2.position + new Vector3(0, cameraHeight, -cameraHeight);
            player2Camera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);

            // mostrar línea divisoria
            if (dividerLine != null)
                dividerLine.SetActive(true);
        }
    }
}
