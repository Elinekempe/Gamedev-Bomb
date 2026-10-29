using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 12f;
    public float minX = -8f;
    public float maxX = 8f;

        void Update()
    {
        // Als de tijd stilstaat (Game Over), doe dan niks
        if (Time.timeScale == 0f) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        Vector3 target = new Vector3(
            Mathf.Clamp(mouse.x, minX, maxX),
            transform.position.y,
            0f);

        transform.position = Vector3.MoveTowards(
            transform.position, target, maxSpeed * Time.deltaTime);
    }
}