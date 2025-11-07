using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    public void PlayGame() => StartCoroutine(StartGame());

    private IEnumerator StartGame()
    {
        // This replaces the current scene instead of stacking them
        SceneManager.LoadScene("GameScreen", LoadSceneMode.Single);
        yield return null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}