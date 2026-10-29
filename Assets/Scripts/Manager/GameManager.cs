using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Spawnen")]
    public GameObject bombPrefab;
    public float spawnInterval = 0.8f;
    public float spawnXMin = -7f;
    public float spawnXMax = 7f;
    public float spawnY = 7f;
    public float bottomY = -6f;

    [Header("Bom-verdeling (gewichten)")]
    public int realWeight = 60;
    public int fakeWeight = 25;
    public int goldWeight = 10;
    public int bigWeight = 5;

    [Header("Stats per type")]
    public BombStats realStats = new BombStats { kind = BombKind.Real, color = new Color(0.2f,0.2f,0.2f), fallSpeed = 2.5f, pointsOnCatch = 10, damageOnMiss = 10 };
    public BombStats fakeStats = new BombStats { kind = BombKind.Fake, color = Color.green,     fallSpeed = 2.5f, damageOnCatch = 10, pointsOnCatch = -5 };
    public BombStats goldStats = new BombStats { kind = BombKind.Gold, color = Color.yellow,    fallSpeed = 4f,   pointsOnCatch = 50, damageOnMiss = 0 };
    public BombStats bigStats  = new BombStats { kind = BombKind.Big,  color = Color.black,     fallSpeed = 1.2f, pointsOnCatch = 20, damageOnMiss = 30, size = new Vector2(1.8f,1.8f) };

    [Header("Spel-status")]
    public int score = 0;
    public int damage = 0;
    public int maxDamage = 100;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI damageText;
    public Image damageBarFill;
    public Image flashOverlay;
    public GameObject gameOverPanel;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    private float timer;
    private bool gameOver = false;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnBomb();
            timer = spawnInterval;
        }
    }

    void SpawnBomb()
    {
        if (bombPrefab == null) return;

        float x = Random.Range(spawnXMin, spawnXMax);
        var go = Instantiate(bombPrefab, new Vector3(x, spawnY, 0f), Quaternion.identity);
        var bomb = go.GetComponent<Bomb>();
        bomb.Init(PickRandomStats(), this, bottomY);
    }

    BombStats PickRandomStats()
    {
        int total = realWeight + fakeWeight + goldWeight + bigWeight;
        int r = Random.Range(0, total);

        if (r < realWeight) return realStats;
        r -= realWeight;
        if (r < fakeWeight) return fakeStats;
        r -= fakeWeight;
        if (r < goldWeight) return goldStats;
        return bigStats;
    }

    public void OnBombCaught(Bomb bomb)
    {
        var s = bomb.stats;
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
        var s = bomb.stats;

        if (s.kind == BombKind.Real || s.kind == BombKind.Big)
        {
            ApplyDamage(s.damageOnMiss);
            Flash(Color.red);
        }
        else if (s.kind == BombKind.Fake)
        {
            AddScore(5);
        }
    }

    void AddScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
        UpdateUI();
    }

    void ApplyDamage(int amount)
    {
        damage = Mathf.Clamp(damage + amount, 0, maxDamage);

        if (cameraShake != null) cameraShake.Shake(0.15f, 0.25f);
        UpdateUI();

        if (damage >= maxDamage) EndGame();
    }

    void Flash(Color c)
    {
        if (flashOverlay == null) return;
        c.a = 0.5f;
        flashOverlay.color = c;
        StartCoroutine(FadeFlash());
    }

    System.Collections.IEnumerator FadeFlash()
    {
        float t = 0.3f;
        float start = 0.3f;
        Color baseColor = flashOverlay.color;

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

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (damageText) damageText.text = "Damage: " + damage + "%";
        if (damageBarFill) damageBarFill.fillAmount = (float)damage / maxDamage;
    }

       void EndGame()
    {
        gameOver = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        // Zet de tijd in het spel stil (alles bevriest)
        Time.timeScale = 0f; 
    }
}