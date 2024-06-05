using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public PolygonCollider2D cameraConfiner;

    private Camera mainCamera; // Kamera bileşenini saklamak için değişken
    private float halfHeight, halfWidth; // Kameranın yarım yüksekliği ve genişliği

    private void Start()
    {
        // Kamera bileşenini başlangıçta al ve sakla
        mainCamera = GetComponent<Camera>();

        // Kameranın yarım yüksekliğini ve genişliğini hesapla
        halfHeight = mainCamera.orthographicSize;
        halfWidth = halfHeight * mainCamera.aspect;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition = ConstrainCameraPosition(desiredPosition);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private Vector3 ConstrainCameraPosition(Vector3 desiredPosition)
    {
        Bounds bounds = cameraConfiner.bounds;
        float xMin = bounds.min.x + halfWidth;
        float xMax = bounds.max.x - halfWidth;
        float yMin = bounds.min.y + halfHeight;
        float yMax = bounds.max.y - halfHeight;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, xMin, xMax);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, yMin, yMax);

        return desiredPosition;
    }
}