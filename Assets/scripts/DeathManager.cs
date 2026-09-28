using System.Collections;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public static DeathManager Instance { get; private set; }

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Interface")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField, Min(0.1f)] private float deathScreenDuration = 2f;

    private bool isDying;

    public bool IsDying => isDying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        deathPanel.SetActive(false);
    }

    public void KillPlayer(GameObject player)
    {
        if (isDying) return;

        isDying = true;
        StartCoroutine(PlayDeathSequence(player));
    }

    private IEnumerator PlayDeathSequence(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement?.SetControlsEnabled(false);

        InventoryManager.Instance?.ClearInventory();

        deathPanel.SetActive(true);

        yield return new WaitForSeconds(deathScreenDuration);

        deathPanel.SetActive(false);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        player.transform.position = respawnPoint.position;

        movement?.SetControlsEnabled(true);
        isDying = false;
    }
}