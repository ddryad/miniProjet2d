using System;
using System.Text;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelPause;
    [SerializeField] private TMP_Text inventoryText;

    private bool paused;

    private void Awake()
    {
        panelPause.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        paused = !paused;

        panelPause.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        if (paused)
        {
            UpdateInventoryDisplay();
        }
    }

    private void UpdateInventoryDisplay()
    {
        if (inventoryText == null || InventoryManager.Instance == null) return;

        StringBuilder builder = new StringBuilder();

        foreach (IngredientType type in Enum.GetValues(typeof(IngredientType)))
        {
            builder.AppendLine($"{type}: {InventoryManager.Instance.GetQuantity(type)}");
        }

        inventoryText.text = builder.ToString();
    }
}