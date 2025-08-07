using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    private Camera mainCamera;

    // Called before the first frame update
    void Start()
    {
        // Cache reference to the main camera for performance
        mainCamera = Camera.main;
    }

    // Called once per frame
    void FixedUpdate()
    {
        if (mainCamera != null)
        {
            // Make the object face the camera
            transform.LookAt(mainCamera.transform);

            // Optionally, keep the object upright (e.g., for UI elements)
            transform.rotation = Quaternion.Euler(0f, 180+transform.rotation.eulerAngles.y, 0f);
        }
    }
}