using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main"); // Laadt de game scene
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene("Tutorial"); // Laadt de uitleg scene
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Laadt het startscherm
    }

    public void QuitGame()
    {
        Application.Quit(); // Sluit de game af (werkt alleen in een build, niet in de editor)
    }
}