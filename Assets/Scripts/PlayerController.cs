using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 12f;
    public Sprite netBombSprite;
    private Camera mainCamera;
    private Renderer playerRenderer;
    private SpriteRenderer playerSpriteRenderer;

    void Awake()
    {
        mainCamera = Camera.main;
        playerRenderer = GetComponent<Renderer>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
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

    void Update()
    {
        // Als de tijd stilstaat (Game Over), doe dan niks
        if (Time.timeScale == 0f) return;
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 mouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        float cameraDepth = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        float leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraDepth)).x;
        float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraDepth)).x;
        float halfWidth = playerRenderer != null ? playerRenderer.bounds.extents.x : 0f;

        Vector3 target = new Vector3(
            Mathf.Clamp(mouse.x, leftEdge + halfWidth, rightEdge - halfWidth),
            transform.position.y,
            0f);

        transform.position = Vector3.MoveTowards(
            transform.position, target, maxSpeed * Time.deltaTime);
    }
}