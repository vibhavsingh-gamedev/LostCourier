
// Attach this to your Main Camera.
// Orbits around the player using mouse input.
// Includes basic collision so camera doesn't clip through walls.

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float smoothSpeed = 12f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;

    private float _yaw, _pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start at player's current rotation
        _yaw = target != null ? target.eulerAngles.y : 0f;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Rotate based on mouse
        _yaw += Input.GetAxis("Mouse X") * sensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0);

        // Check for wall collision using raycast
        float actualDistance = distance;
        Vector3 pivotPos = target.position + Vector3.up * height;
        Vector3 desiredDir = rot * Vector3.back;

        if (Physics.Raycast(pivotPos, desiredDir, out RaycastHit hit, distance, collisionLayers))
            actualDistance = Mathf.Max(hit.distance - 0.2f, 0.5f);

        Vector3 finalPos = pivotPos + desiredDir * actualDistance;

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, finalPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(pivotPos);
    }
}