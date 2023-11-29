using System;
using UnityEngine;

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
