using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    public void PlayGame() => StartCoroutine(StartGame());

    private IEnumerator StartGame()
    {
        SceneManager.LoadScene("GameScreen");
        yield return null; // wait one frame for scene load

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}