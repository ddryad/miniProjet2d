using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlingshotProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float launchMultiplier = 20f;
    [SerializeField, Min(0.1f)] private float maxDragDistance = 2f;

    private Rigidbody2D rb;
    private Vector3 anchorPosition;
    private bool dragging;
    private bool launched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        anchorPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (launched) return;
        dragging = true;
    }

    private void OnMouseDrag()
    {
        if (!dragging) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        Vector3 offset = mouseWorldPosition - anchorPosition;
        if (offset.magnitude > maxDragDistance)
        {
            offset = offset.normalized * maxDragDistance;
        }

        transform.position = anchorPosition + offset;
    }

    private void OnMouseUp()
    {
        if (!dragging) return;

        dragging = false;
        launched = true;

        Vector3 launchDirection = anchorPosition - transform.position;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = launchDirection * launchMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!launched) return;

        if (other.CompareTag("Pot"))
        {
            MiniGameManager.Instance?.OnProjectileLanded(true);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            MiniGameManager.Instance?.OnProjectileLanded(false);
            Destroy(gameObject);
        }
    }
}