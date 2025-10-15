using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // Load your GameScene
        SceneManager.LoadScene("GameScreen");

        // Wait one frame so the scene finishes loading
        yield return null;

        // Lock the cursor to the center, but keep it visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        Debug.Log("✅ Cursor locked in center but visible");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}