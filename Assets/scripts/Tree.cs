using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tree : MonoBehaviour
{
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField, Min(0f)] private float cooldownDuration = 3f;
    [SerializeField] private Sprite cooldownSprite;

    private SpriteRenderer sprite;
    private Sprite normalSprite;
    private float nextAvailableTime;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        normalSprite = sprite.sprite;
    }

    public void Hit()
    {
        if (Time.time < nextAvailableTime) return;

        nextAvailableTime = Time.time + cooldownDuration;

        Vector3 spawnPosition = dropPoint != null ? dropPoint.position : transform.position;
        Instantiate(collectablePrefab, spawnPosition, Quaternion.identity);

        StartCoroutine(PlayCooldownVisual());
    }

    private IEnumerator PlayCooldownVisual()
    {
        sprite.sprite = cooldownSprite;

        yield return new WaitForSeconds(cooldownDuration);

        sprite.sprite = normalSprite;
    }
}