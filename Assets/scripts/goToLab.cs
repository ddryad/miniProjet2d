using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class goToLab : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (!autre.CompareTag("Player"))
            return;

        int sceneActuelleIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneActuelleIndex);
    }
}