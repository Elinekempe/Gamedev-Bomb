using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bomb : MonoBehaviour
{
    public BombStats stats;

    [Header("Sprites per bomtype")]
    public Sprite realSprite;
    public Sprite fakeSprite;
    public Sprite goldSprite;
    public Sprite bigSprite;
    public float rotationZ = -90f;

    [Header("Geluid")]
    public AudioClip catchSound;
    public AudioClip missSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private GameManager gm;
    private float bottomY;
    private SpriteRenderer sr;
    private float horizontalDirection;

    public void Init(BombStats stats, GameManager manager, float bottomY)
    {
        this.stats = stats;
        this.gm = manager;
        this.bottomY = bottomY;

        sr = GetComponent<SpriteRenderer>();
        ApplyVisuals();
        transform.localScale = new Vector3(stats.size.x, stats.size.y, 1f);
        transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        horizontalDirection = Random.value < 0.5f ? -1f : 1f;
    }

    private void ApplyVisuals()
    {
        Sprite selectedSprite = null;
        Color fallbackColor = Color.white;

        switch (stats.kind)
        {
            case BombKind.Real:
                selectedSprite = realSprite;
                fallbackColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                break;
            case BombKind.Fake:
                selectedSprite = fakeSprite;
                fallbackColor = new Color(0.7f, 1f, 0.7f, 1f);
                break;
            case BombKind.Gold:
                selectedSprite = goldSprite;
                fallbackColor = new Color(1f, 0.85f, 0.25f, 1f);
                break;
            case BombKind.Big:
                selectedSprite = bigSprite;
                fallbackColor = new Color(1f, 0.65f, 0.65f, 1f);
                break;
        }

        if (selectedSprite != null)
        {
            sr.sprite = selectedSprite;
            sr.color = Color.white;
        }
        else
        {
            sr.color = fallbackColor;
        }
    }

    void Update()
    {
        Vector3 movement = new Vector3(
            horizontalDirection * stats.horizontalSpeed,
            -stats.fallSpeed,
            0f) * Time.deltaTime;
        transform.Translate(movement, Space.World);
        KeepInsideScreen();

        if (transform.position.y < bottomY)
        {
            if (stats != null && stats.kind != BombKind.Fake)
            {
                PlaySound(missSound);
            }

            if (gm != null) gm.OnBombMissed(this);
            Destroy(gameObject);
        }
    }

    private void KeepInsideScreen()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        float cameraDepth = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        float leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraDepth)).x;
        float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraDepth)).x;
        float halfWidth = sr.bounds.extents.x;

        Vector3 position = transform.position;
        if (position.x - halfWidth <= leftEdge)
        {
            position.x = leftEdge + halfWidth;
            horizontalDirection = 1f;
        }
        else if (position.x + halfWidth >= rightEdge)
        {
            position.x = rightEdge - halfWidth;
            horizontalDirection = -1f;
        }

        transform.position = position;
    }

       void OnTriggerEnter2D(Collider2D other)
    {
        // Check of we de speler raken
        if (other.CompareTag("Player"))
        {
            PlaySound(catchSound);

            // Vertel de GameManager dat we gevangen zijn
            if (gm != null) gm.OnBombCaught(this);
            
            // Vernietig de bom (hij verdwijnt dus!)
            Destroy(gameObject);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }
}