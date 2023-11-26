// MovementController.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private AgentController agentController;
    public int batchSize = 100; // Number of agents to update per batch
    public int updateInterval = 3; // Update every 3 frames
    public int currentFrame = 0;
    public int currentBatchStart = 0;

    private Camera mainCamera;
    public LayerMask agentLayer;
    public LayerMask goalLayer;
    public GameObject selectedAgent;
    public Agent[] playerAgents;
    public PlayerConfig playerConfig;

    private readonly Dictionary<GameObject, Identifier> identifierCache = new Dictionary<GameObject, Identifier>();

    public void Initialize(AgentController agentController)
    {
        this.agentController = agentController;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, agentLayer);
            if (hit.collider != null)
            {
                Identifier identifier = GetIdentifier(hit.collider.gameObject);

                string agentId = identifier.Id; // Assuming the name is the agent ID
                if (agentController.agents.TryGetValue(agentId, out Agent agent))
                {
                    // Toggle manual control
                    agent.ToggleManualControl();
                    agent.SetSelected(agent.IsUnderManualControl);
                    selectedAgent = agent.LinkedGameObject;
                }
            }
        }
    }


    void FixedUpdate()
    {
        // Increment frame counter
        currentFrame++;
        if (currentFrame >= updateInterval)
        {
            // Reset frame counter
            currentFrame = 0;

            // Process a batch of agents
            ProcessAgentBatch();

            // Prepare for next batch
            currentBatchStart += batchSize;
            if (currentBatchStart >= agentController.agents.Count)
            {
                currentBatchStart = 0; // Reset to the first batch
            }
        }
    }
    private void ProcessAgentBatch()
    {
        int count = 0;
        foreach (var agent in agentController.agents.Values.Skip(currentBatchStart))
        {
            if (count >= batchSize) break; // Process only a limited number of agents per batch

            if (agent.IsUnderManualControl)
            {
                ApplyManualControl(agent);
            }
            else
            {
                ApplyMovement(agent);
                ApplyRotation(agent);
            }

            count++;
        }
    }


    private void ApplyManualControl(Agent agent)
    {
        Rigidbody2D rb = agent.Rigidbody; // Use the cached Rigidbody
        if (rb == null) return;

        // Apply thrust on 'W' key press
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(agent.LinkedGameObject.transform.up * agent.MaxSpeed, ForceMode2D.Force);
        }

        float rotationInput = 0.0f;
        if (Input.GetKey(KeyCode.A)) rotationInput = -1.0f; // Left rotation
        else if (Input.GetKey(KeyCode.D)) rotationInput = 1.0f; // Right rotation

        rb.AddTorque(rotationInput * agent.MaxRotationSpeed);
    }

    private void ApplyMovement(Agent agent)
    {
        Rigidbody2D rb = agent.Rigidbody; // Use the cached Rigidbody
                                          // Assuming LastOutputs[0] is for thrust and LastOutputs[1] is for rotation
        float thrust = agent.LastOutputs[0] * agent.MaxSpeed;
        Vector2 thrustDirection = agent.LinkedGameObject.transform.up * thrust;

        rb.AddForce(thrustDirection, ForceMode2D.Force);

        // Debug lines can remain as is, since they don't affect performance significantly
    }

    private void ApplyRotation(Agent agent)
    {
        float rotation = agent.LastOutputs[1] * agent.MaxRotationSpeed;

        Rigidbody2D rb = agent.Rigidbody; // Use the cached Rigidbody

        rb.AddTorque(rotation);
    }


    private Identifier GetIdentifier(GameObject gameObject)
    {
        if (!identifierCache.TryGetValue(gameObject, out Identifier identifier))
        {
            identifier = gameObject.GetComponent<Identifier>();
            identifierCache[gameObject] = identifier;
        }
        return identifier;
    }
}

//PlayerConfig class inherits from ScriptableObject
public class PlayerConfig : ScriptableObject
{
    public float MaxSpeed = 10f;
    public float MaxRotationSpeed = 10f;
    public float Thrust = 10f;
    public float Rotation = 1f;
}