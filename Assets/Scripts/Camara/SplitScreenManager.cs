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

    [Header("Parámetros Generales")]
    public float splitDistance;
    public float cameraHeight;
    public float cameraAngle;

    [Header("Cámara Compartida (Main Camera)")]
    public Vector3 sharedCameraOffset = new Vector3(0, 10, -10);

    [Tooltip("Rotación personalizada de la cámara compartida (Euler angles).")]
    public Vector3 sharedCameraRotation = new Vector3(30, 0, 0);

    [Header("UI")]
    public GameObject dividerLine;

    public void AssignPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
    }

    void Update()
    {
        if (player1 == null || player2 == null)
            return;

        float distance = Vector3.Distance(player1.position, player2.position);

        if (distance < splitDistance)
        {
            // --- Cámara compartida ---
            mainCamera.gameObject.SetActive(true);
            player1Camera.gameObject.SetActive(false);
            player2Camera.gameObject.SetActive(false);

            Vector3 midPoint = (player1.position + player2.position) / 2f;
            mainCamera.transform.position = midPoint + sharedCameraOffset;
            mainCamera.transform.rotation = Quaternion.Euler(sharedCameraRotation);

            if (dividerLine != null)
                dividerLine.SetActive(false);
        }
        else
        {
            // --- Split Screen ---
            mainCamera.gameObject.SetActive(false);
            player1Camera.gameObject.SetActive(true);
            player2Camera.gameObject.SetActive(true);

            player1Camera.transform.position = player1.position + new Vector3(0, cameraHeight, -cameraHeight);
            player1Camera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);

            player2Camera.transform.position = player2.position + new Vector3(0, cameraHeight, -cameraHeight);
            player2Camera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);

            if (dividerLine != null)
                dividerLine.SetActive(true);
        }
    }
}
