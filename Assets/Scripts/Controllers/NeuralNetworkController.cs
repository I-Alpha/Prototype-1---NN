using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class NeuralNetworkManager
{
    private INeuralNetwork MockNeuralNetwork;
    private Dictionary<string, INeuralNetwork> neuralNetworks = new Dictionary<string, INeuralNetwork>();

    public NeuralNetworkManager()
    {
        // Initialize the NeuralNetworkSimulator
        MockNeuralNetwork = new MockNeuralNetwork();
    }

    // Method to add a neural network to the manager
    public INeuralNetwork CreateNeuralNetwork(string nnType = "mock", Dictionary<string, object> parameters = null)
    {
        INeuralNetwork neuralNetwork;

        if (nnType == "mock")
        {
            // Create a mock neural network
            neuralNetwork = new MockNeuralNetwork();
        }
        else
        {
            if (parameters != null && parameters.ContainsKey("layers") && parameters.ContainsKey("size") && parameters.ContainsKey("inputSize") && parameters.ContainsKey("outputSize"))
            {
                // Extract 'layers' and 'size' from the parameters dictionary
                int[] layers = parameters["layers"] as int[];
                int inputSize = Convert.ToInt32(parameters["inputSize"]);
                int outputSize = Convert.ToInt32(parameters["outputSize"]);

                if (layers != null)
                {
                    neuralNetwork = new NeuralNetwork(layers, inputSize, outputSize);
                }
                else
                {
                    throw new ArgumentException("Invalid parameters for Neural Network creation");
                }
            }
            else
            {
                throw new ArgumentException("Required parameters for Neural Network creation not provided");
            }
        }

        neuralNetworks.Add(neuralNetwork.Id, neuralNetwork);
        return neuralNetwork;
    }

    // Method to add a neural network to the manager
    public INeuralNetwork GetNeuralNetworkById(string id)
    {
        return neuralNetworks.FirstOrDefault(entry => entry.Key == id).Value;
    }
    public void AddNeuralNetwork(INeuralNetwork network)
    {
        neuralNetworks.Add(network.Id, network);
    }
    public void RemoveNeuralNetworkById(string id)
    {
        var network = neuralNetworks.FirstOrDefault(entry => entry.Key == id).Value;
        if (network != null)
        {
            neuralNetworks.Remove(network.Id);
            Debug.Log($"Neural Network withID {id} removed.");
        }
        else
        {
            Debug.LogError($"Neural Network with ID {id} not found.");
        }
    }
    public void RemoveNeuralNetwork(INeuralNetwork nn)
    {

        var network = neuralNetworks.FirstOrDefault(entry => entry.Value == nn).Value;
        if (network != null)
        {
            neuralNetworks.Remove(network.Id);
            Debug.Log($"Neural Network withID {network.Id} removed.");
        }
        else
        {
            Debug.LogError($"Neural Network with ID {network.Id} not found.");
        }
    }


    public void SaveNeuralNetworks(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonData = JsonUtility.ToJson(new SerializableNeuralNetworkList(neuralNetworks), true);
        File.WriteAllText(path, jsonData);
        Debug.Log("Neural Networks saved to " + path);
    }
    public void SaveNeuralNetwork(string fileName)
    {
        // Assuming we are saving a mock neural network for now
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonData = JsonUtility.ToJson(MockNeuralNetwork, true);
        File.WriteAllText(path, jsonData);
        Debug.Log("Neural Network saved to " + path);
    }
    public void LoadNeuralNetworks(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            SerializableNeuralNetworkList loadedData = JsonUtility.FromJson<SerializableNeuralNetworkList>(jsonData);

            foreach (var neuralNetwork in loadedData.ToNeuralNetworkList())
            {
                neuralNetworks.Add(neuralNetwork.Id, neuralNetwork);
            }

            Debug.Log("Neural Networks loaded from " + path);
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }
    public void LoadNeuralNetwork(string fileName)
    {
        // For loading, you will need to adapt this method to reconstruct the neural network
        // Currently, this is a placeholder implementation
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            // Deserialize and reconstruct the neural network
            Debug.Log("Neural Network loaded from " + path);
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }
}

public interface INeuralNetwork
{
    string Id { get; }
    float[] FeedForward(float[] inputs);
    // Other common methods and properties  

    public INeuralNetwork Clone();

    public string CustomName { get; set; }

    public int InputSize { get; set; }
    public int OutputSize { get; set; }
}

public class NeuralNetwork : INeuralNetwork
{
    public string Id { get; private set; }
    public string DefaultName { get; private set; }
    public string CustomName { get; set; }
    public int InputSize { get; set; }
    public int OutputSize { get; set; }

    private static int instanceCounter = 0;

    public NeuralNetwork(int[] layers, int inputSize, int outputSize)
    {
        Id = Guid.NewGuid().ToString();
        DefaultName = $"nn-type-{++instanceCounter}";
        InitializeNeurons(layers);
        InitializeWeights(layers);
        InputSize = inputSize;
        OutputSize = outputSize;
    }

    public INeuralNetwork Clone()
    {
        // Implement a deep copy of the neural network
        throw new NotImplementedException();
    }

    private void InitializeNeurons(int[] layers)
    {
        // Initialize neuron layers based on the provided architecture
    }

    private void InitializeWeights(int[] layers)
    {
        // Initialize weight layers with random values
    }

    public float[] FeedForward(float[] inputs)
    {
        // Implement the forward propagation logic
        throw new NotImplementedException();
    }

    private float ActivationFunction(float value)
    {
        // Implement an activation function, e.g., ReLU or sigmoid
        throw new NotImplementedException();
    }

    // Additional methods for mutation, crossover, etc.
}

public class MockNeuralNetwork : INeuralNetwork
{

    private static int instanceCounter = 0;

    public string CustomName { get; set; }
    public string Id { get; private set; }
    public int InputSize { get; set; }
    public int OutputSize { get; set; }
    public string DefaultName { get; private set; }


    private System.Random random = new System.Random(); // Random instance

    public MockNeuralNetwork(int inputSize = 2, int ouputSize = 2)
    {
        DefaultName = $"nn-type-mock-{++instanceCounter}";
        Id = Guid.NewGuid().ToString();
        InputSize = inputSize;
        OutputSize = ouputSize;
    }
    public float[] FeedForward(float[] inputs) {
        float[] outputs = new float[2];
        outputs[0] = inputs[0] * 2;
        outputs[1] = inputs[1] * 2;
        return outputs;
    }
    public float[] FeedForward(float[] inputs, Rigidbody2D agentRigidbody)
    {
        float distance = inputs[0]; // Distance to the goal
        float direction = inputs[1]; // Direction to the goal in degrees

        float[] outputs = new float[2];

        // Proportional gains
        float rotationGain = 1.0f; // Adjust for sensitivity
        float thrustGain = 1.0f; // Adjust for thrust sensitivity
        float counterTorqueFactor = 0.3f; // Factor to apply counter torque, adjust as needed

        // Normalize direction to a value between -1 and 1
        float normalizedDirection = Mathf.Clamp(direction / 180.0f, -1.0f, 1.0f);

        // Counter torque based on the current angular velocity
        float angularVelocity = agentRigidbody.angularVelocity;
        float counterTorque = Mathf.Sign(angularVelocity) * Mathf.Min(Mathf.Abs(angularVelocity) * counterTorqueFactor, rotationGain);

        // Adjust the rotation output
        outputs[1] = normalizedDirection * rotationGain - counterTorque;

        // Thrust control
        float directionTolerance = 10.0f; // Tighter tolerance for thrust direction
        if (Mathf.Abs(direction) <= directionTolerance)
        {
            // Proportional thrust control
            outputs[0] = Mathf.Clamp(thrustGain * (1f - Mathf.Clamp01(distance)), 0f, 1f);
        }
        else
        {
            outputs[0] = 0f; // No thrust if not facing the goal within tolerance
        }

        return outputs;
    }



    public INeuralNetwork Clone()
    {
        // Implement a deep copy of the neural network
        throw new NotImplementedException();
    }
    private void InitializeNeurons(int[] layers)
    {
        // Initialize neuron layers based on the provided architecture
    }

    private void InitializeWeights(int[] layers)
    {
        // Initialize weight layers with random values
    }
    private float ActivationFunction(float value)
    {
        // Implement an activation function, e.g., ReLU or sigmoid
        throw new NotImplementedException();
    }

    float[] GenerateRandomInputs(int size)
    {
        float[] inputs = new float[size];
        for (int i = 0; i < size; i++)
        {
            inputs[i] = ((float)random.NextDouble() * 2f) - 1f; // Random float between 0 and 1
        }
        return inputs;
    }
}

[Serializable]
public class SerializableNeuralNetwork
{
    // Serializable representations of the neural network's data
    public List<float[]> neurons;
    public List<List<float[]>> weights;

    // Other necessary properties for serialization

    public SerializableNeuralNetwork(INeuralNetwork neuralNetwork)
    {

        // Convert the neural network's data into a serializable format
        throw new NotImplementedException();
    }

    public INeuralNetwork ToNeuralNetwork()
    {
        // Reconstruct and return a NeuralNetwork instance from the serialized data
        throw new NotImplementedException();
    }
}

[Serializable]
public class SerializableNeuralNetworkList
{
    public List<SerializableNeuralNetwork> networks;

    public SerializableNeuralNetworkList(Dictionary<string, INeuralNetwork> neuralNetworks)
    {
        networks = new List<SerializableNeuralNetwork>();
        foreach (var (id, network) in neuralNetworks)
        {
            networks.Add(new SerializableNeuralNetwork(network));
        }
    }

    public List<INeuralNetwork> ToNeuralNetworkList()
    {
        List<INeuralNetwork> list = new List<INeuralNetwork>();
        foreach (var serializableNetwork in networks)
        {
            list.Add(serializableNetwork.ToNeuralNetwork());
        }
        return list;
    }
}
