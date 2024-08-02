using UnityEngine;

namespace FeonY.RainbowForest 
{
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this to change the speed of camera movement

    void Update()
    {
        // Get the horizontal and vertical inputs (WASD keys)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on inputs
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // Move the camera based on the calculated direction
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
}