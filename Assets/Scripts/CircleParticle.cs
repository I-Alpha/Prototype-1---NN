using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class CircleParticle : MonoBehaviour
{
    public float radius = 1.0f;
    public float bounceStrength = 1.0f;
    public Vector2 velocity;
    private Vector2 screenSize;

    void Start()
    {
        // Calculate screen bounds
        screenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    void Update()
    {
        // Update position based on velocity
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Bounce off screen edges
        CheckBounds();
    }

    void CheckBounds()
    {
        if (transform.position.x - radius < -screenSize.x || transform.position.x + radius > screenSize.x)
        {
            velocity.x *= -bounceStrength;
        }
        if (transform.position.y - radius < -screenSize.y || transform.position.y + radius > screenSize.y)
        {
            velocity.y *= -bounceStrength;
        }
    }
}
