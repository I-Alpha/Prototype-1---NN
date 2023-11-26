using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

public class EnvironmentController : MonoBehaviour
{
    private float updateTimer = 0.0f;
    private const float updateInterval = 0.5f; // Update every 0.5 seconds
    private AgentController agentController;
    private SpawnController spawnController;
    public List<GameObject> goals;
    public int initialGoalAmount = 2;
    public int initialAgentAmount = 1;

    public bool Player1;

    public void Initialize(AgentController agentController, SpawnController spawnController)
    {
        this.agentController = agentController;
        this.spawnController = spawnController;
    }
    public void SetupEnvironment()
    {
        goals = new List<GameObject>();
        //disctionary pair of strin and int, key and value. for amounts fo objects to spawn

        Dictionary<string, int> parameters = new Dictionary<string, int>
        {
            { "player", Player1 ? 1: 0 },
            { "agents", initialAgentAmount },
            { "goals", initialGoalAmount }
        };

        spawnController.SpawnObjects(parameters);
    }


    void FixedUpdate()
    {
        // Increment the timer
        updateTimer += Time.deltaTime;

        // Check if it's time to update
        if (updateTimer >= updateInterval)
        {
            HandlePlayer();
            HandleAgents();

            // Reset the timer after the update
            updateTimer = 0.0f;
        }
    }

    private void HandlePlayer()
    {

    }
    private void HandleAgents()
    {
        // Iterate through all agents managed by AgentController
        foreach (var agentId in agentController.agents.Keys)
        {
            Agent agent = agentController.agents[agentId];

            // Update the neural network with the current sensory inputs
            agent.Think();

            // If there are any goals in the environment, process them
            if (goals.Count > 0)
            {
                // Find the nearest goal to the current agent
                GameObject nearestGoal = FindNearestGoal(agent.LinkedGameObject);

                // Calculate the distance to the nearest goal
                float nearestGoalDistance = GetDistanceToNearestGoal(nearestGoal, agent.LinkedGameObject);
                // Calculate the angle to the nearest goal in relation to the agent's current orientation
                float nearestGoalDirection = getDirectionToGoal(nearestGoal, agent.LinkedGameObject);

                // Set the sensory inputs for the agent's neural network
                agent.CurrentInputs[0] = nearestGoalDistance;
                agent.CurrentInputs[1] = nearestGoalDirection;

                // Check if the agent has reached the goal
                if (nearestGoalDistance < 1f) // someThreshold should be a small value indicating "closeness"
                {
                    // Reward the agent for reaching the goal
                    agent.Reward(0.2f);

                    // Remove the goal from the environment
                    goals.Remove(nearestGoal);
                    Destroy(nearestGoal);

                    // Optionally, spawn a new goal at a random position within the boundaries
                    spawnController.SpawnGoalOnPosition(
                        new Vector2(Random.Range(Globals.playPenBoundaries.bounds.min.x, Globals.playPenBoundaries.bounds.max.x),
                                    Random.Range(Globals.playPenBoundaries.bounds.min.y, Globals.playPenBoundaries.bounds.max.y))
                    );
                }
                else
                {
                    // If the agent is closer to the goal than last time, give a small reward
                    if (agent.LastInputs[0] > nearestGoalDistance)
                    {
                        agent.Reward(0.01f);
                    }
                    // If the agent is further from the goal than last time, give a small penalty
                    else if (agent.LastInputs[0] < nearestGoalDistance)
                    {
                        agent.Reward(-0.01f);
                    }
                }

                // Save the current distance for comparison in the next update
                agent.LastInputs[0] = nearestGoalDistance;
            }
        }
    }
    void OnDrawGizmos()
    {
        if (agentController != null)
        {
            foreach (var agentId in agentController.agents.Keys)
            {
                Agent agent = agentController.agents[agentId];
                GameObject agentGameObject = agent.LinkedGameObject;

                if (agentGameObject != null)
                {
                    GameObject nearestGoal = FindNearestGoal(agentGameObject);
                    if (nearestGoal != null)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(agentGameObject.transform.position, nearestGoal.transform.position);

                        float distance = Vector3.Distance(agentGameObject.transform.position, nearestGoal.transform.position);
                        Handles.Label((agentGameObject.transform.position + nearestGoal.transform.position) * 0.5f, $"Distance: {distance:F2}");

                        Gizmos.color = Color.white;
                        Vector3 forward = agentGameObject.transform.up * distance;
                        Gizmos.DrawLine(agentGameObject.transform.position, agentGameObject.transform.position + forward);
                        Handles.Label(agentGameObject.transform.position + forward, "Facing");

                        float currentAngle = agentGameObject.transform.eulerAngles.z;
                        Handles.Label(agentGameObject.transform.position, $"Current Angle: {currentAngle:F2}");

                        // Use getDirectionToGoal for "Should Face" direction calculation
                        float angleToGoal = getDirectionToGoal(nearestGoal, agentGameObject);
                        Vector3 shouldFaceDirection = Quaternion.Euler(0, 0, currentAngle + angleToGoal) * Vector3.up * 2; // Adjust multiplier for line length
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(agentGameObject.transform.position, agentGameObject.transform.position + shouldFaceDirection);
                        Handles.Label(agentGameObject.transform.position + shouldFaceDirection, $"Target Angle: {angleToGoal:F2}");
                    }
                }
            }
        }
    }


    private float getDirectionToGoal(GameObject goal, GameObject agent)
    {
        Vector3 directionToGoal = (goal.transform.position - agent.transform.position).normalized;
        Vector3 agentForward = agent.transform.up; // Assuming 'up' is the forward direction

        // Calculate angle in degrees from the agent's forward direction to the direction to the goal
        float angle = Vector3.SignedAngle(agentForward, directionToGoal, Vector3.forward);
        return angle;
    }



    private float GetDistanceToNearestGoal(GameObject goal, GameObject agent)
    {
        // Corrected distance calculation
        return Vector3.Distance(agent.transform.position, goal.transform.position);
    }


    private Vector2 getGoalPosition(GameObject goal)
    {
        //return distance to nearest goal or false if no goals
        return goal
            .transform.position;
    }

    GameObject FindNearestGoal(GameObject agent)
    {
        //return nearest goal or false if no goals
        return goals
            .OrderBy(goal => Vector3.Distance(agent.transform.position, goal.transform.position))
            .FirstOrDefault();
    }

    // Additional methods and logic as needed
    public class SpriteGenerator
    {
        public Sprite GenerateSprite(string[] genes)
        {
            // Use the genes to determine the appearance of the sprite
            // This could involve selecting different sprite parts, colors, etc.
            return null;
        }
    }


}
