using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class LevelGate : MonoBehaviour
{
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private string miniGameSceneName;
    [SerializeField] private string nextSceneName;

    [Header("Final Level")]
    [SerializeField] private bool isFinalLevel;
    [SerializeField] private GameObject endPanel;

    private SpriteRenderer sprite;
    private bool isOpen;
    private bool playerInRange;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = closedSprite;
    }

    private void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);

        if (MiniGameManager.IsCompleted(miniGameSceneName))
            Open();
    }

    private void Update()
    {
        if (isOpen && playerInRange && Time.timeScale > 0f && Input.GetKeyDown(KeyCode.W))
        {
            EnterGate();
        }
    }

    public void Open()
    {
        isOpen = true;
        sprite.sprite = openSprite;
    }

    private void EnterGate()
    {
        if (isFinalLevel)
        {
            EndGame();
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void EndGame()
    {
        if (endPanel != null)
            endPanel.SetActive(true);

        PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null)
            pauseMenu.enabled = false;

        Time.timeScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}