using Unity.Entities;

public struct BrainComponent : IComponentData
{
    public ClosestEntitiesData closestEntitiesData;
    // Other sensory data fields...

    // You might also include fields that represent the state of the agent
    // or outputs from the neural network
}