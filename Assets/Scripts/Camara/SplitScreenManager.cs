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
    public float splitDistance = 15f;
    public float cameraHeight = 10f;
    public float cameraAngle = 30f;

    [Header("Cámara Compartida (Main Camera)")]
    public Vector3 sharedCameraOffset = new Vector3(0, 10, -10);
    public Vector3 sharedCameraRotation = new Vector3(30, 0, 0);

    [Header("Suavizado y Transiciones")]
    public float hysteresis = 2f;
    [Tooltip("Súbele a este valor (ej. 0.5 o 0.8) si quieres que el efecto de alejarse/acercarse sea más lento y dramático.")]
    public float cameraSmoothTime = 0.3f;
    public float uiFadeSpeed = 5f;

    [Header("UI")]
    public CanvasGroup dividerCanvasGroup;

    private bool isSplit = false;

    // Variables de referencia para el SmoothDamp
    private Vector3 mainCamVelocity = Vector3.zero;
    private Vector3 p1CamVelocity = Vector3.zero;
    private Vector3 p2CamVelocity = Vector3.zero;

    public void AssignPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
    }

    void Start()
    {
        mainCamera.transform.rotation = Quaternion.Euler(sharedCameraRotation);
        player1Camera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
        player2Camera.transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);

        if (dividerCanvasGroup != null)
        {
            dividerCanvasGroup.alpha = 0f;
            dividerCanvasGroup.gameObject.SetActive(true);
        }
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        float distance = Vector3.Distance(player1.position, player2.position);

        // Guardamos el estado anterior para saber si justo en este frame hubo un cambio
        bool previousSplitState = isSplit;

        if (isSplit && distance < (splitDistance - hysteresis))
            isSplit = false;
        else if (!isSplit && distance > (splitDistance + hysteresis))
            isSplit = true;

        // --- LA MAGIA DEL ZOOM EMPIEZA AQUÍ ---
        if (isSplit != previousSplitState) // Si el estado acaba de cambiar...
        {
            if (isSplit)
            {
                // Se acaban de separar: Las cámaras individuales nacen donde está la principal.
                // Así, bajarán suavemente hacia los jugadores.
                player1Camera.transform.position = mainCamera.transform.position;
                player2Camera.transform.position = mainCamera.transform.position;

                // Reiniciamos la velocidad para que el SmoothDamp empiece limpio
                p1CamVelocity = Vector3.zero;
                p2CamVelocity = Vector3.zero;
            }
            else
            {
                // Se acaban de juntar: La principal nace en medio de donde estaban las divididas.
                // Así, subirá suavemente para ver a ambos.
                Vector3 midPointCam = (player1Camera.transform.position + player2Camera.transform.position) / 2f;
                mainCamera.transform.position = midPointCam;

                mainCamVelocity = Vector3.zero;
            }
        }
        // --- FIN DE LA MAGIA ---

        if (!isSplit)
        {
            mainCamera.gameObject.SetActive(true);
            player1Camera.gameObject.SetActive(false);
            player2Camera.gameObject.SetActive(false);

            Vector3 midPoint = (player1.position + player2.position) / 2f;
            Vector3 targetPosition = midPoint + sharedCameraOffset;

            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref mainCamVelocity, cameraSmoothTime);

            if (dividerCanvasGroup != null)
                dividerCanvasGroup.alpha = Mathf.Lerp(dividerCanvasGroup.alpha, 0f, Time.deltaTime * uiFadeSpeed);
        }
        else
        {
            mainCamera.gameObject.SetActive(false);
            player1Camera.gameObject.SetActive(true);
            player2Camera.gameObject.SetActive(true);

            Vector3 p1TargetPos = player1.position + new Vector3(0, cameraHeight, -cameraHeight);
            Vector3 p2TargetPos = player2.position + new Vector3(0, cameraHeight, -cameraHeight);

            player1Camera.transform.position = Vector3.SmoothDamp(player1Camera.transform.position, p1TargetPos, ref p1CamVelocity, cameraSmoothTime);
            player2Camera.transform.position = Vector3.SmoothDamp(player2Camera.transform.position, p2TargetPos, ref p2CamVelocity, cameraSmoothTime);

            if (dividerCanvasGroup != null)
                dividerCanvasGroup.alpha = Mathf.Lerp(dividerCanvasGroup.alpha, 1f, Time.deltaTime * uiFadeSpeed);
        }
    }
}