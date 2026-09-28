using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    private static readonly HashSet<string> completedScenes = new HashSet<string>();

    [Header("Objective")]
    [SerializeField] private IngredientType ingredientType;
    [SerializeField, Min(1)] private int objectiveLandings = 3;

    [Header("Spawning")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Interface")]
    [SerializeField] private TMP_Text ingredientCountText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject successPanel;
    [SerializeField, Min(0.1f)] private float successScreenDuration = 3f;
    [SerializeField] private string platformerSceneName;

    private int landingsCount;

    public static bool IsCompleted(string sceneName)
    {
        return completedScenes.Contains(sceneName);
    }

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
        if (successPanel != null)
            successPanel.SetActive(false);

        UpdateScoreDisplay();
        SpawnNextProjectile();
    }

    public void OnProjectileLanded(bool success)
    {
        if (success)
        {
            landingsCount++;
            UpdateScoreDisplay();

            if (landingsCount >= objectiveLandings)
            {
                CompleteMiniGame();
                return;
            }
        }

        SpawnNextProjectile();
    }

    public void LeaveLab()
    {
        SceneManager.LoadScene(platformerSceneName);
    }

    private void CompleteMiniGame()
    {
        completedScenes.Add(SceneManager.GetActiveScene().name);

        if (successPanel != null)
            StartCoroutine(ShowSuccessScreen());
    }

    private IEnumerator ShowSuccessScreen()
    {
        successPanel.SetActive(true);

        yield return new WaitForSeconds(successScreenDuration);

        successPanel.SetActive(false);
    }

    private void SpawnNextProjectile()
    {
        bool hasIngredient = InventoryManager.Instance != null &&
            InventoryManager.Instance.RemoveIngredient(ingredientType);

        UpdateIngredientDisplay();

        if (!hasIngredient)
        {
            Debug.Log("Out of ingredients.");
            return;
        }

        Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
    }

    private void UpdateIngredientDisplay()
    {
        if (ingredientCountText == null || InventoryManager.Instance == null) return;

        ingredientCountText.text = $"{ingredientType}: {InventoryManager.Instance.GetQuantity(ingredientType)}";
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText == null) return;

        scoreText.text = $"Score: {landingsCount}/{objectiveLandings}";
    }
}