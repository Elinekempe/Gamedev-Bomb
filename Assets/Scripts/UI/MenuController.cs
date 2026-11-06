using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private const string DefaultGameplayScene = "Main";
    private const string DefaultTutorialScene = "Tutorial";
    private const string DefaultMainMenuScene = "MainMenu";

    [Header("Scene-overgangen")]
    [SerializeField] private string gameplayScene;
    [SerializeField] private string tutorialScene;
    [SerializeField] private string mainMenuScene;

    public void StartGame()
    {
        // De startknop opent de gameplay-scene.
        SceneManager.LoadScene(GetSceneName(gameplayScene, DefaultGameplayScene));
    }

    public void OpenTutorial()
    {
        // De tutorial is vanuit het main menu opnieuw te openen.
        SceneManager.LoadScene(GetSceneName(tutorialScene, DefaultTutorialScene));
    }

    public void BackToMenu()
    {
        // Deze callback wordt gebruikt door de terugknop in de tutorial.
        SceneManager.LoadScene(GetSceneName(mainMenuScene, DefaultMainMenuScene));
    }

    private string GetSceneName(string configuredScene, string fallbackScene)
    {
        // Een lege Inspectorwaarde mag een menu-overgang niet laten crashen.
        return string.IsNullOrWhiteSpace(configuredScene) ? fallbackScene : configuredScene;
    }

    public void QuitGame()
    {
        // Application.Quit heeft alleen effect in een gemaakte build.
        Application.Quit(); // Sluit de game af (werkt alleen in een build, niet in de editor)
    }
}