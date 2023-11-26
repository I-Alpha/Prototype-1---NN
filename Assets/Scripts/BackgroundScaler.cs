using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    public SpriteRenderer backgroundSpriteRenderer; // Assign in Inspector

    void Start()
    {
        ScaleBackgroundToScreen();
    }

    void ScaleBackgroundToScreen()
    {
        if (backgroundSpriteRenderer == null) return;

        // Convert screen dimensions from pixels to world space
        float screenHeightInWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y -
                                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        float screenWidthInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x -
                                   Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        // Get the dimensions of the sprite in world space
        float spriteWidth = backgroundSpriteRenderer.sprite.bounds.size.x;
        float spriteHeight = backgroundSpriteRenderer.sprite.bounds.size.y;

        // Calculate the scaling factor to fit the screen
        Vector3 scale = transform.localScale;
        scale.x = screenWidthInWorld / spriteWidth;
        scale.y = screenHeightInWorld / spriteHeight;
        transform.localScale = scale;
    }

}
