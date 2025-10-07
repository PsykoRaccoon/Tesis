using UnityEngine;

public class TwoPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // velocidad de movimiento
    public bool isPlayer1 = true; // true = jugador 1, false = jugador 2

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (isPlayer1)
        {
            // Controles jugador 1 - WASD
            if (Input.GetKey(KeyCode.W)) moveZ += 1f;
            if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
            if (Input.GetKey(KeyCode.D)) moveX += 1f;
            if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        }
        else
        {
            // Controles jugador 2 - Flechas
            if (Input.GetKey(KeyCode.UpArrow)) moveZ += 1f;
            if (Input.GetKey(KeyCode.DownArrow)) moveZ -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) moveX += 1f;
            if (Input.GetKey(KeyCode.LeftArrow)) moveX -= 1f;
        }

        // mover al jugador
        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}
