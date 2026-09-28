using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class FondDefilant : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer modelPanel;
    [SerializeField] private float scrollSpeed = 0.5f;

    private readonly List<SpriteRenderer> panels = new List<SpriteRenderer>();
    private float width;
    private float originX;
    private float centerLocalX;
    private float scrolled;
    private Vector3 modelPosition;

    private void Start()
    {
        if (!targetCamera) targetCamera = Camera.main;

        if (!targetCamera || !targetCamera.orthographic || !modelPanel || !modelPanel.sprite)
        {
            Debug.LogError("FondDefilant : assign an orthographic camera and a panel with a sprite.", this);
            enabled = false;
            return;
        }

        width = modelPanel.bounds.size.x;

        if (width <= 0.0001f)
        {
            enabled = false;
            return;
        }

        modelPosition = modelPanel.transform.position;
        originX = modelPanel.bounds.center.x;
        centerLocalX = originX - modelPosition.x;
        panels.Add(modelPanel);

        Refresh();
    }

    private void LateUpdate()
    {
        scrolled += scrollSpeed * Time.deltaTime;
        Refresh();
    }

    private void Refresh()
    {
        float halfView = targetCamera.orthographicSize * targetCamera.aspect;
        int required = Mathf.CeilToInt(2f * halfView / width) + 3;

        while (panels.Count < required)
        {
            SpriteRenderer copy = Instantiate(modelPanel, modelPanel.transform.parent);
            copy.name = "Recycled panel " + panels.Count;
            panels.Add(copy);
        }

        int count = panels.Count;
        float origin = originX + scrolled;
        int first = Mathf.FloorToInt((targetCamera.transform.position.x - halfView - origin) / width);

        for (int i = 0; i < count; i++)
        {
            int index = first + i;
            int slot = ((index % count) + count) % count;

            Vector3 p = modelPosition;
            p.x = origin + index * width - centerLocalX;
            panels[slot].transform.position = p;
        }
    }
}