using UnityEngine;

public class LineShaderController : MonoBehaviour
{
    private MeshRenderer meshRenderer; // Reference to the SpriteRenderer component

    public bool animationOn = false;
    public float growthSpeed = 1.5f;
    public float lineThickness = .3f;
    public float circleRadius = 1.0f;

    private float gameTime = 0f;
    private float pauseTime = 0f;

    public Transform initialPositionTransform;
    private Vector2 initialPosition = new Vector2(0.5f, 0.5f); // Set the initial position here
    private Vector2 initialVelocity = new Vector2(0.5f, 0.5f);

    private Vector2 position = new Vector2(0.5f, 0.5f); // Set the initial position here
    private Vector2 velocity = new Vector2(0.5f, 0.5f);

    public bool branchOn = false;
    public float branchGrowthSpeed = 1.0f;
    public Vector2 branchDirection = new Vector2(1, 0); // Example: Rightward
    void Start()
    {
        initialPositionTransform = GetComponent<Transform>();
        if (initialPositionTransform != null)
        {
            initialPosition = initialPositionTransform.position;
        }
        // Get the SpriteRenderer component attached to this GameObject
        meshRenderer = GetComponent<MeshRenderer>();
        pauseTime = gameTime;
        // Set _StartTime in the shader
    }

    void Update()
    {
        // Reset start time when AnimationOn transitions from 0 to 1

        if (animationOn)
        {
            gameTime = Time.time - pauseTime;
        }
        else
        {
            gameTime = 0;
            pauseTime = Time.time;
        }

        if (branchOn || Input.GetKeyDown(KeyCode.B))
        {
            meshRenderer.material.SetFloat("_BranchStartPosition", position.x);
            meshRenderer.material.SetVector("_BranchDirection", new Vector4(branchDirection.x, branchDirection.y, 0, 0));
            meshRenderer.material.SetFloat("_BranchGrowthSpeed", branchGrowthSpeed);
        }
        // Set _StartTime in the shader
        meshRenderer.material.SetFloat("_GameTime", gameTime);
        meshRenderer.material.SetVector("_InitialPosition", new Vector4(initialPosition.x, initialPosition.y, 0, 0));
        meshRenderer.material.SetFloat("_PositionX", position.x);
        meshRenderer.material.SetFloat("_PositionY", position.y);
        meshRenderer.material.SetFloat("_GrowthSpeed", growthSpeed);
        meshRenderer.material.SetFloat("_CircleRadius", circleRadius * 0.01f);
        meshRenderer.material.SetFloat("_LineThickness", lineThickness);
    }
}
