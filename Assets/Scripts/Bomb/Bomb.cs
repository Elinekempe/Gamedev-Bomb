using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
    private BombStats stats;
    public BombStats Stats => stats;

    [Header("Sprites per bomtype")]
    [SerializeField] private Sprite realSprite;
    [SerializeField] private Sprite fakeSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite bigSprite;
    [SerializeField] private float rotationZ = -90f;

    [Header("Geluid")]
    [SerializeField] private AudioClip catchSound;
    [SerializeField] private AudioClip missSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    private GameManager gm;
    private float bottomY;
    private SpriteRenderer sr;
    private float horizontalDirection;

    public void Init(BombStats stats, GameManager manager, float bottomY)
    {
        // De GameManager geeft iedere instantie zijn type, regels en verwijzing terug.
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
        Color fallbackColor = stats.color;

        // Het type bepaalt welke sprite en fallbackkleur deze bom gebruikt.
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

    private void Update()
    {
        // Horizontale en verticale beweging worden gecombineerd tot een vaste valrichting.
        Vector3 movement = new Vector3(
            horizontalDirection * stats.horizontalSpeed,
            -stats.fallSpeed,
            0f) * Time.deltaTime;
        transform.Translate(movement, Space.World);
        KeepInsideScreen();

        if (transform.position.y < bottomY)
        {
            // Een bom die onder het speelveld komt telt als gemist en wordt verwijderd.
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

        // De bom keert om zodra de rand van de camera wordt geraakt.
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Alleen een botsing met de speler telt als een gevangen bom.
        if (other.CompareTag("Player"))
        {
            PlaySound(catchSound);

            // De GameManager verwerkt score en eventuele schade.
            if (gm != null) gm.OnBombCaught(this);
            Destroy(gameObject);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        // AudioClip is optioneel; zonder clip blijft de gameplay gewoon werken.
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }
}