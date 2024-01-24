
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public Vector3 offset;  // Offset vector

    void Update()
    {
        // Update the camera's position to follow the player
        transform.position = player.position + offset;

        // Make the camera rotate as the player rotates
        transform.rotation = player.rotation;
    }
}
