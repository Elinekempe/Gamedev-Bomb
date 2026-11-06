using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string DefaultMainMenuScene = "MainMenu";

    [Header("Spel-status")]
    [SerializeField] private int score = 0;
    [SerializeField] private int damage = 0;
    [SerializeField] private int maxDamage = 100;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Image damageBarFill;
    [SerializeField] private Image flashOverlay;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;

    [Header("Scene-overgangen")]
    [SerializeField] private string mainMenuScene;

    [SerializeField] private BombSpawner bombSpawner;
    private bool gameOver = false;

    private void Start()
    {
        // Het game-overpanel is pas zichtbaar nadat de speler verloren heeft.
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (gameOver) return;

        if (bombSpawner != null)
        {
            bombSpawner.Tick(this, Time.deltaTime);
        }
    }

    public void OnBombCaught(Bomb bomb)
    {
        // Een gevangen bom gebruikt de puntenwaarde van zijn eigen type.
        var s = bomb.Stats;
        AddScore(s.pointsOnCatch);

        if (s.kind == BombKind.Fake)
        {
            ApplyDamage(s.damageOnCatch);
            Flash(Color.red);
        }
        else if (s.kind == BombKind.Gold)
        {
            Flash(new Color(1f, 0.85f, 0f, 0.5f));
        }
    }

    public void OnBombMissed(Bomb bomb)
    {
        // Een gemiste bom kan naast puntenverlies ook schade veroorzaken.
        var s = bomb.Stats;
        AddScore(s.pointsOnMiss);

        if (s.kind == BombKind.Real || s.kind == BombKind.Big)
        {
            ApplyDamage(s.damageOnMiss);
            Flash(Color.red);
        }
    }

    private void AddScore(int amount)
    {
        score += amount;
        // De score mag nooit negatief worden weergegeven.
        if (score < 0) score = 0;
        UpdateUI();
    }

    private void ApplyDamage(int amount)
    {
        // Clamp voorkomt dat de schade onder nul of boven de maximale waarde komt.
        damage = Mathf.Clamp(damage + amount, 0, maxDamage);

        if (cameraShake != null) cameraShake.Shake(0.15f, 0.25f);
        UpdateUI();

        if (damage >= maxDamage) EndGame();
    }

    private void Flash(Color c)
    {
        if (flashOverlay == null) return;
        // Een korte kleurflits geeft visuele feedback bij een goede of slechte actie.
        c.a = 0.5f;
        flashOverlay.color = c;
        StartCoroutine(FadeFlash());
    }

    private System.Collections.IEnumerator FadeFlash()
    {
        float t = 0.3f;
        float start = 0.3f;
        Color baseColor = flashOverlay.color;

        // De alpha wordt geleidelijk afgebouwd zodat de flits niet abrupt verdwijnt.
        while (t > 0f)
        {
            t -= Time.deltaTime;
            var col = baseColor;
            col.a = Mathf.Lerp(0f, baseColor.a, t / start);
            flashOverlay.color = col;
            yield return null;
        }

        var clear = baseColor;
        clear.a = 0f;
        flashOverlay.color = clear;
    }

    private void UpdateUI()
    {
        // Alle HUD-elementen worden vanuit dezelfde methode bijgewerkt.
        if (scoreText) scoreText.text = "Score: " + score;
        if (damageText) damageText.text = "Damage: " + damage + "%";
        if (damageBarFill) damageBarFill.fillAmount = (float)damage / maxDamage;
    }

    private void EndGame()
    {
        gameOver = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            ShowFinalScore();
        }

        // Bevries de gameplay, maar laat de UI-knoppen bruikbaar.
        Time.timeScale = 0f; 
    }

    private void ShowFinalScore()
    {
        if (scoreText == null || gameOverPanel == null) return;

        // De bestaande scoretekst wordt naar het game-overpanel verplaatst.
        RectTransform scoreRect = scoreText.rectTransform;
        scoreText.transform.SetParent(gameOverPanel.transform, false);
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = new Vector2(0f, 140f);
        scoreRect.sizeDelta = new Vector2(420f, 70f);
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.text = "EIND SCORE: " + score;
        scoreText.transform.SetAsLastSibling();
    }

    public void ReturnToMainMenu()
    {
        // Time.timeScale moet worden hersteld voordat de volgende scene start.
        Time.timeScale = 1f;
        string sceneToLoad = string.IsNullOrWhiteSpace(mainMenuScene)
            ? DefaultMainMenuScene
            : mainMenuScene;
        SceneManager.LoadScene(sceneToLoad);
    }
}