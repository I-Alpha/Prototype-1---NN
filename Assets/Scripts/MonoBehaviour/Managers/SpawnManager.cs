using Borgs;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject goalPrefab;

    public GameObject agentsNode;
    public GameObject goalsNode;

    public Transform spawnPoint;

    public float spawnDelay = 0.5f;
    private float spawnTimer = 0f;

    //need an select for spawn areas (playpen, world, etc
    public BoundaryType goalSpawnArea;
    public BoundaryType agentSpawnArea;

    private bool isSpawning = false;
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
        BaseBoundaries2D baseBoundaries2D = goalSpawnArea == BoundaryType.PlayPen ? Globals.playPenBoundaries : Globals.worldBoundaries;
        for (int i = 0; i < amount; i++)
        {
            SpawnGoalOnPosition(baseBoundaries2D.GetRandomPositionFromCache());
        }
    }


    public void SpawnAgentsOnTargeter(int amount)
    {
        BaseBoundaries2D baseBoundaries2D = agentSpawnArea == BoundaryType.PlayPen ? Globals.playPenBoundaries : Globals.worldBoundaries;

        for (int i = 0; i < amount; i++)
        {
            SpawnAgentOnPosition(
           baseBoundaries2D.GetRandomPositionFromCache()
                                          );
        }
    }

    public void SpawnAgentOnPosition(Vector2 Position)
    {
        GameObject agentGameObject = Instantiate(agentPrefab, (Vector3)Position, Quaternion.identity);
        //add borg to agentsNode
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
        if (hit.collider != null && hit.collider.gameObject.tag == "Goal" && hit.collider.gameObject != spawnPoint.gameObject)
        {
            Destroy(hit.collider.gameObject);
        }
    }


    public void SpawnGoalOnPosition(Vector2 position)
    {
        GameObject newGoal = Instantiate(goalPrefab, position, Quaternion.identity);

        // Add goal to goalsNode and EnvironmentController's list
        newGoal.transform.SetParent(goalsNode.transform);
    }

}
