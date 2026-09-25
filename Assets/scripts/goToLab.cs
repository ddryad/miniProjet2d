using UnityEngine;
using UnityEngine.SceneManagement;

public class LabDoor : MonoBehaviour
{
    [SerializeField] private string MiniGame;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneManager.LoadScene(MiniGame);
    }
}