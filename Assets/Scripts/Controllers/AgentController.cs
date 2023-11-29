using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.ParticleSystem;

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
