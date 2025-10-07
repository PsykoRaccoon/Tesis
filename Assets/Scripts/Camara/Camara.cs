using UnityEngine;

public class LOLCamera : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 20f; // velocidad de desplazamiento
    public float borderThickness = 10f; // grosor del borde para mover con el mouse
    public Vector2 moveLimits = new Vector2(50, 50); // límite del movimiento en X/Z

    [Header("Zoom")]
    public float scrollSpeed = 200f; // velocidad de zoom
    public float minY = 20f; // altura mínima
    public float maxY = 60f; // altura máxima

    private void Update()
    {
        Vector3 pos = transform.position;

        // ---- Movimiento con teclado ----
        if (Input.GetKey("w")) pos.z += moveSpeed * Time.deltaTime;
        if (Input.GetKey("s")) pos.z -= moveSpeed * Time.deltaTime;
        if (Input.GetKey("d")) pos.x += moveSpeed * Time.deltaTime;
        if (Input.GetKey("a")) pos.x -= moveSpeed * Time.deltaTime;

        // ---- Movimiento con mouse (bordes) ----
        if (Input.mousePosition.y >= Screen.height - borderThickness)
            pos.z += moveSpeed * Time.deltaTime;
        if (Input.mousePosition.y <= borderThickness)
            pos.z -= moveSpeed * Time.deltaTime;
        if (Input.mousePosition.x >= Screen.width - borderThickness)
            pos.x += moveSpeed * Time.deltaTime;
        if (Input.mousePosition.x <= borderThickness)
            pos.x -= moveSpeed * Time.deltaTime;

        // ---- Zoom con scroll ----
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.deltaTime;

        // ---- Limitar área y altura ----
        pos.x = Mathf.Clamp(pos.x, -moveLimits.x, moveLimits.x);
        pos.z = Mathf.Clamp(pos.z, -moveLimits.y, moveLimits.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
