using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SpawnController : MonoBehaviour
{
    public GameObject agentPrefab; // Assign your ball prefab in the Inspector
    public GameObject goalPrefab; // Assign your ball prefab in the 
    public GameObject agentsNode; // Assign your ball prefab in the Inspector
    public GameObject goalsNode; // Assign your ball prefab in the 
    public Transform targeter; // Assign the targeter transform in the Inspector
    public float spawnDelay = 0.5f;
    private float spawnTimer = 0f;

    //need an select for spawn areas (playpen, world, etc
    public enum SpawnArea { PlayPen, World };
    public SpawnArea goalSpawnArea;
    public SpawnArea agentSpawnArea;





    private bool isSpawning = false;
    private AgentController agentController;
    private EnvironmentController environmentController;


    public void Initialize(AgentController agentController, EnvironmentController environmentController)
    {
        this.agentController = agentController;
        this.environmentController = environmentController;
    }
    public void SpawnObjects(Dictionary<string, int> parameters)
    {

        SpawnAgentsOnTargeter(parameters["agents"]);
        SpawnGoalsRandomly(parameters["goals"]);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && !isSpawning && spawnTimer <= 0f)
        {
            isSpawning = true;
            SpawnGoalOnClick();
        }

        if (Input.GetKey(KeyCode.LeftAlt) & Input.GetMouseButtonDown(0))
        {
            DestroyGoalOnClick();
        }
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnDelay)
            {
                spawnTimer = 0f;
                isSpawning = false;
            }
        }
    }
    public void SpawnGoalsRandomly(int amount)
    {
        //use enum SpawnArea o select spawn area
        BaseBoundaries2D baseBoundaries2D = goalSpawnArea == SpawnArea.PlayPen ? Globals.playPenBoundaries : Globals.worldBoundaries;
        for (int i = 0; i < amount; i++)
        {
            SpawnGoalOnPosition(baseBoundaries2D.GetRandomPositionWithinBounds());
        }
    }


    public void SpawnAgentsOnTargeter(int amount)
    {
        BaseBoundaries2D baseBoundaries2D = agentSpawnArea == SpawnArea.PlayPen ? Globals.playPenBoundaries : Globals.worldBoundaries;

        for (int i = 0; i < amount; i++)
        {
            SpawnAgentOnPosition(
           baseBoundaries2D.GetRandomPositionWithinBounds()
                                          );
        }
    }

    public void SpawnAgentOnPosition(Vector2 Position)
    {
        GameObject agentGameObject = Instantiate(agentPrefab, (Vector3)Position, Quaternion.identity);
        // Create and link an agent to the new sprite
        agentController.CreateAgent("agent", agentGameObject); // CreateAgent returns the neural network ID
        //add agent to agentsNode
        agentGameObject.transform.SetParent(agentsNode.transform);
        agentGameObject.transform.localPosition = Position;

    }
    public void SpawnGoalOnClick(int amount = 1)
    {
        // Convert the mouse position to a world point
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f; // Set this to the appropriate distance from the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        SpawnGoalOnPosition(worldPosition);
    }

    public void DestroyGoalOnClick()
    {
        // Convert the mouse position to a world point
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f; // Set this to the appropriate distance from the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Perform a raycast to detect the clicked object
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Check if the raycast hits a goal and it’s not the targeter
        if (hit.collider != null && hit.collider.gameObject.tag == "Goal" && hit.collider.gameObject != targeter.gameObject)
        {
            Destroy(hit.collider.gameObject);
        }
    }


    public void SpawnGoalOnPosition(Vector2 position)
    {
        GameObject newGoal = Instantiate(goalPrefab, position, Quaternion.identity);

        // Add goal to goalsNode and EnvironmentController's list
        newGoal.transform.SetParent(goalsNode.transform);
        environmentController.goals.Add(newGoal);
    }

}
