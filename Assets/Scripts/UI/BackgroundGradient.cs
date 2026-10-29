using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundGradient : MonoBehaviour
{
    public Color topColor = new Color(0.1f, 0.1f, 0.15f);
    public Color bottomColor = new Color(0.4f, 0.1f, 0.1f);
    
    [ContextMenu("Update Gradient Nu")] // Hiermee kun je hem in de editor verversen
    public void GenerateGradient()
    {
        int width = 256;
        int height = 256;
        Texture2D tex = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            Color color = Color.Lerp(bottomColor, topColor, t);
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();

        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    void Start()
    {
        GenerateGradient();
    }
}