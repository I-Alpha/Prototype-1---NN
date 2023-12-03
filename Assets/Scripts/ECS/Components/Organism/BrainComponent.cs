using Unity.Entities;

namespace Borgs
{
    public struct BrainComponent : IComponentData
    {
        public ClosestEntitiesData closestEntitiesData;
        // Other sensory data fields...

        // You might also include fields that represent the state of the borg
        // or outputs from the neural network
    }
}