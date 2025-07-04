using UnityEngine;
// Keeps the player GameObject positioned and oriented relative to the AR camera.
// Useful for syncing the player avatar with the camera's position and direction in AR space.
public class PlayerFollowCamera : MonoBehaviour
{
    public Transform arCamera;
    public float yOffset = 1f;
 // Hides the player’s mesh renderer at startup (optional for invisible tracking object)
    void Start()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.enabled = false;
        }
    }
    // Updates the player’s position and orientation each physics frame
    void FixedUpdate()
    {
        if (arCamera == null) return;
        // Follow the AR camera’s position with a downward Y offset
        Vector3 newPos = arCamera.position;
        newPos.y -= yOffset; 
        transform.position = newPos;
        // Rotate the player to face the same horizontal direction as the camera
        Vector3 forward = arCamera.forward;
        forward.y = 0;// Keep rotation only on horizontal axis
        if (forward.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}
