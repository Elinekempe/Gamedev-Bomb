using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bomb : MonoBehaviour
{
    public BombStats stats;

    [Header("Geluid")]
    public AudioClip catchSound;
    public AudioClip missSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private GameManager gm;
    private float bottomY;
    private SpriteRenderer sr;

    public void Init(BombStats stats, GameManager manager, float bottomY)
    {
        this.stats = stats;
        this.gm = manager;
        this.bottomY = bottomY;

        sr = GetComponent<SpriteRenderer>();
        sr.color = stats.color;
        transform.localScale = new Vector3(stats.size.x, stats.size.y, 1f);
    }

    void Update()
    {
        transform.Translate(Vector3.down * stats.fallSpeed * Time.deltaTime);

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