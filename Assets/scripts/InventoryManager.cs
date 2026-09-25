using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly Dictionary<IngredientType, int> quantities = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddIngredient(IngredientType type, int amount = 1)
    {
        if (amount <= 0) return;

        quantities[type] = quantities.GetValueOrDefault(type) + amount;
    }

    public bool RemoveIngredient(IngredientType type, int amount = 1)
    {
        if (amount <= 0) return false;

        if (!quantities.ContainsKey(type) || quantities[type] < amount)
        {
            return false;
        }

        quantities[type] -= amount;
        return true;
    }

    public int GetQuantity(IngredientType type)
    {
        return quantities.TryGetValue(type, out int value) ? value : 0;
    }
}