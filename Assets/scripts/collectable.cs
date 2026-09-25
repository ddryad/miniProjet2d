using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectable : MonoBehaviour
{
    [SerializeField] private IngredientType ingredientType;
    [SerializeField, Min(1)] private int quantity = 10;
    [SerializeField, Min(0f)] private float pickupDelay = 0.5f;

    private float collectibleTime;

    private void Awake()
    {
        collectibleTime = Time.time + pickupDelay;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < collectibleTime) return;

        InventoryManager.Instance?.AddIngredient(ingredientType, quantity);
        Destroy(gameObject);
    }
}