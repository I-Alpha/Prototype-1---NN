using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "AgentConfig", menuName = "Configurations/AgentConfig")]
public class AgentConfig : ScriptableObject
{
    public float MaxSpeed;
    public float MaxRotationSpeed;
    public string behaviorType;
    public bool canReproduce;
    public float size;
    public float energy;
    public float maxEnergy;
    // Additional properties
    public float MaxAcceleration;
    
    public float MaxAngularAcceleration;
    public float MaxAngularVelocity;
        
}

// In Unity Editor, create instances like "FlyConfig" and "PlantConfig" and set their properties.

public class AgentController : MonoBehaviour
{
    private NeuralNetworkManager neuralNetworkManager;
    public readonly Dictionary<string, Agent> agents = new Dictionary<string, Agent>();
    public AgentConfig basicAgentConfig;
    public AgentConfig basicConfig;

    //on initialization
    public AgentController()
    {
        neuralNetworkManager = new NeuralNetworkManager();
    }

    #region Agent Functions

    public Agent CreateAgent(string type, GameObject sprite)
    {
        INeuralNetwork neuralNetwork = neuralNetworkManager.CreateNeuralNetwork(); //no parameters for now means mock neural network
        Agent newAgent = new Agent(neuralNetwork);

        // Determine the configuration based on the type
        AgentConfig selectedConfig = (type == "agent") ? basicAgentConfig : basicConfig; // 'otherConfig' is another AgentConfig instance

        newAgent.Initialize(selectedConfig, sprite);
        agents.Add(newAgent.Id, newAgent);
        return newAgent;
    }


    public void UpdateAgentSprite(string agentId, GameObject newSprite)
    {
        if (agents.TryGetValue(agentId, out Agent agent))
        {
            // Update the agent's linked GameObject and re-initialize the Rigidbody and SpriteRenderer
            agent.LinkedGameObject = newSprite;
            agent.Rigidbody = newSprite.GetComponent<Rigidbody2D>();
            agent.SpriteRenderer = newSprite.GetComponent<SpriteRenderer>();
        }
    }
    #endregion

    // Additional methods for agent management
}

public class Agent
{
    public string[] Genes;
    public string[] parents { get; set; }
    public string Id { get; set; }
    public string CustomName { get; set; }
    public float[] CurrentInputs { get; set; }
    public float[] LastInputs { get; set; }

    public float MaxRotationSpeed { get; set; } = .2f;
    public float MaxSpeed { get; set; } = 10f;
    public float[] LastOutputs { get; set; }
    public string DefaultName { get; private set; }
    public INeuralNetwork NeuralNetwork { get; private set; }
    public GameObject LinkedGameObject { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    public float Fitness { get; set; }
    public bool IsUnderManualControl { get; set; } = false;

    private static int instanceCounter = 0;

    public GeneticData genetics;
    public GeneticTraits traits;

    public Agent(INeuralNetwork neuralNetwork)
    {
        NeuralNetwork = neuralNetwork;
        CurrentInputs = new float[neuralNetwork.InputSize];
        LastInputs = new float[neuralNetwork.InputSize];
        LastOutputs = new float[neuralNetwork.OutputSize];

        Id = Guid.NewGuid().ToString();
        DefaultName = $"agent-type-{++instanceCounter}";
        Fitness = 0;
    }

    // Initialize method
    public void Initialize(AgentConfig config, GameObject linkedGameObject)
    {

        MaxSpeed = config.MaxSpeed;
        MaxRotationSpeed = config.MaxRotationSpeed;
        // Setup linked GameObject
        LinkedGameObject = linkedGameObject;
        LinkedGameObject.GetComponent<Identifier>().SetId(Id);
        Rigidbody = linkedGameObject.GetComponent<Rigidbody2D>();
        SpriteRenderer = linkedGameObject.GetComponent<SpriteRenderer>();
    }

    public void InitializeGenetics(GeneticData geneticsData)
    {
        genetics = geneticsData;
        traits = genetics.Decode();
        // Update agent appearance and behavior based on traits
    }
    public void Think()
    {
        LastInputs = CurrentInputs;
        LastOutputs = NeuralNetwork.FeedForward(CurrentInputs);
    }

    public void Reward(float rewardsPoints)
    {
        Fitness += rewardsPoints;
    }

    public void ToggleManualControl()
    {
        IsUnderManualControl = !IsUnderManualControl;
        SpriteRenderer.color = IsUnderManualControl ? Color.blue : Color.white;
    }
    public void SetSelected(bool isSelected)
    {
        SpriteRenderer.color = isSelected ? Color.green : Color.white;
    }

}
