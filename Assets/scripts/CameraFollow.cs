using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0.1f)] private float smoothSpeed = 5f;

    private float fixedY;
    private float fixedZ;

    private void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, fixedY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}