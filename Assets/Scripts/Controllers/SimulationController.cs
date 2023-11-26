using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public AgentController agentController;
    public EnvironmentController environmentController;
    public MovementController movementController;
    public SpawnController spawnController;

    void Start()
    {
        // Inject dependencies
        environmentController.Initialize(agentController, spawnController);
        spawnController.Initialize(agentController, environmentController);
        movementController.Initialize(agentController);

        // Start game logic
        environmentController.SetupEnvironment();
    }
}
