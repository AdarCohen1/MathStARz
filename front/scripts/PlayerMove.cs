using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls basic player movement in the direction of the camera's forward vector.
// The player moves only when the screen is touched or mouse is held down.
public class PlayerMove : MonoBehaviour
{
    public float speed = 1.5f; // מהירות תזוזה
    public float groundOffset = 0.05f;
    public float raycastDistance = 5f;

    void Update()
    {
        MoveInCameraDirection();
    }
    // Moves the player forward relative to camera direction when mouse/touch is active.
    // Adjusts Y-position based on ground height and rotates to match camera Y-angle.
    void MoveInCameraDirection()
    {
        if (Camera.main == null) return;

        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        //  Move only when the mouse is held (or touch is detected on mobile)
        if (Input.GetMouseButton(0)) 
        {
            transform.position += forward * speed * Time.deltaTime;
        }

//  Adjust height based on the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, raycastDistance))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + groundOffset, transform.position.z);
        }

        // Rotate player to match camera's Y angle
        float cameraY = Camera.main.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, cameraY, 0);
    }

}
