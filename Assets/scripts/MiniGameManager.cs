using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [Header("Objective")]
    [SerializeField] private IngredientType ingredientType = IngredientType.Leaf;
    [SerializeField, Min(1)] private int objectiveLandings = 3;

    [Header("Spawning")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    private int landingsCount;

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
        SpawnNextProjectile();
    }

    public void OnProjectileLanded(bool success)
    {
        if (success)
        {
            landingsCount++;

            if (landingsCount >= objectiveLandings)
            {
                Debug.Log("Mini-game complete!");
                return;
            }
        }

        SpawnNextProjectile();
    }

    private void SpawnNextProjectile()
    {
        bool hasIngredient = InventoryManager.Instance != null &&
            InventoryManager.Instance.RemoveIngredient(ingredientType);

        if (!hasIngredient)
        {
            Debug.Log("Out of ingredients.");
            Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            return;

        }

        Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
    }
}