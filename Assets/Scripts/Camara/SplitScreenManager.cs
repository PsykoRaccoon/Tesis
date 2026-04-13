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

    [Header("Configuración de Vista (Rotación)")]
    [Tooltip("Inclinación de la cámara hacia abajo.")]
    public float rotationX = 40f;
    [Tooltip("Giro de la cámara. 90 grados mira hacia la derecha.")]
    public float rotationY = 90f;

    [Header("Offsets de Posición (¡Modifica esto en Play Mode!)")]
    [Tooltip("Posición relativa al punto medio cuando los jugadores están JUNTOS.")]
    public Vector3 sharedCameraOffset = new Vector3(-15, 10, 0);
    [Tooltip("Posición relativa a cada jugador cuando están SEPARADOS (suele ser más cerca).")]
    public Vector3 splitCameraOffset = new Vector3(-15, 10, 0);

    [Header("Reglas de Pantalla Dividida")]
    public float splitDistance = 15f;
    [Tooltip("Margen para evitar parpadeos si caminan en el límite.")]
    public float hysteresis = 2f;

    [Header("Suavizado y Transiciones")]
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
        // Aplicamos la rotación inicial de 90 grados a todas las cámaras
        Quaternion fixedRotation = Quaternion.Euler(rotationX, rotationY, 0);
        mainCamera.transform.rotation = fixedRotation;
        player1Camera.transform.rotation = fixedRotation;
        player2Camera.transform.rotation = fixedRotation;

        if (dividerCanvasGroup != null)
        {
            dividerCanvasGroup.alpha = 0f;
            dividerCanvasGroup.gameObject.SetActive(true);
        }
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        // Mantenemos la rotación fija todo el tiempo por si algo intenta cambiarla
        Quaternion fixedRotation = Quaternion.Euler(rotationX, rotationY, 0);
        mainCamera.transform.rotation = fixedRotation;
        player1Camera.transform.rotation = fixedRotation;
        player2Camera.transform.rotation = fixedRotation;

        float distance = Vector3.Distance(player1.position, player2.position);

        // Guardamos el estado anterior para la animación de Zoom
        bool previousSplitState = isSplit;

        // Lógica de Histéresis
        if (isSplit && distance < (splitDistance - hysteresis))
            isSplit = false;
        else if (!isSplit && distance > (splitDistance + hysteresis))
            isSplit = true;

        // --- MAGIA DEL ZOOM (Transición) ---
        if (isSplit != previousSplitState)
        {
            if (isSplit)
            {
                // Al separarse: Las individuales nacen donde está la principal
                player1Camera.transform.position = mainCamera.transform.position;
                player2Camera.transform.position = mainCamera.transform.position;

                p1CamVelocity = Vector3.zero;
                p2CamVelocity = Vector3.zero;
            }
            else
            {
                // Al juntarse: La principal nace entre las dos individuales
                Vector3 midPointCam = (player1Camera.transform.position + player2Camera.transform.position) / 2f;
                mainCamera.transform.position = midPointCam;

                mainCamVelocity = Vector3.zero;
            }
        }
        // --- FIN MAGIA DEL ZOOM ---

        if (!isSplit)
        {
            // MODO CÁMARA COMPARTIDA
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
            // MODO PANTALLA DIVIDIDA
            mainCamera.gameObject.SetActive(false);
            player1Camera.gameObject.SetActive(true);
            player2Camera.gameObject.SetActive(true);

            Vector3 p1TargetPos = player1.position + splitCameraOffset;
            Vector3 p2TargetPos = player2.position + splitCameraOffset;

            player1Camera.transform.position = Vector3.SmoothDamp(player1Camera.transform.position, p1TargetPos, ref p1CamVelocity, cameraSmoothTime);
            player2Camera.transform.position = Vector3.SmoothDamp(player2Camera.transform.position, p2TargetPos, ref p2CamVelocity, cameraSmoothTime);

            if (dividerCanvasGroup != null)
                dividerCanvasGroup.alpha = Mathf.Lerp(dividerCanvasGroup.alpha, 1f, Time.deltaTime * uiFadeSpeed);
        }
    }
}