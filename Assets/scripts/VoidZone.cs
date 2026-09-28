using UnityEngine;

public class VoidZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DeathManager.Instance?.KillPlayer(other.gameObject);
    }
}