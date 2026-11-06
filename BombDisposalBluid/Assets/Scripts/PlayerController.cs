using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float verticalMovementRange = 1.5f;
    [SerializeField] private Sprite netBombSprite;
    private Camera mainCamera;
    private Renderer playerRenderer;
    private SpriteRenderer playerSpriteRenderer;
    private float startingY;

    private void Awake()
    {
        // De beginhoogte bepaalt het onderste punt waar de speler kan komen.
        mainCamera = Camera.main;
        playerRenderer = GetComponent<Renderer>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        startingY = transform.position.y;
        SetSpriteMaterial();

        if (netBombSprite != null)
        {
            playerSpriteRenderer.sprite = netBombSprite;
        }
    }

    private void SetSpriteMaterial()
    {
        Shader spriteShader = Shader.Find("Sprites/Default");
        if (spriteShader != null)
        {
            playerSpriteRenderer.material = new Material(spriteShader);
        }
    }

    private void Update()
    {
        // Als de tijd stilstaat (Game Over), doe dan niks
        if (Time.timeScale == 0f) return;
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        // Zet de muispositie om naar wereldcoordinaten voor de 2D-game.
        Vector3 mouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        float cameraDepth = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        // ViewportToWorldPoint vertaalt de camerabreedte naar wereldcoordinaten.
        float leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraDepth)).x;
        float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraDepth)).x;
        float halfWidth = playerRenderer != null ? playerRenderer.bounds.extents.x : 0f;

        Vector3 target = new Vector3(
            // Clamp houdt de speler binnen het speelveld en beperkt de verticale range.
            Mathf.Clamp(mouse.x, leftEdge + halfWidth, rightEdge - halfWidth),
            Mathf.Clamp(mouse.y, startingY, startingY + verticalMovementRange),
            0f);

        transform.position = Vector3.MoveTowards(
            transform.position, target, maxSpeed * Time.deltaTime);
    }
}