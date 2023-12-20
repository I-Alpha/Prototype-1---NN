using Unity.Entities;
using Unity.Mathematics;

namespace Borgs
{
    public struct PhysicsTransformComponent : IComponentData
    {
        public float speed; // Forward speed
        public float rotationSpeed;
        public float acceleration;
        public float angularAcceleration;
        public bool isDirty;
        public float angularVelocity;

        public float maxSpeed;
        public float maxRotationSpeed;
        public float maxAcceleration;
        public float maxAngularAcceleration;
        public float maxAngularVelocity;


        public float borgAttractionDistance; // Define the distance for attraction
        public float goalAttractionDistance; // Define the distance for attraction
        public float attractionMultiplier; // Define the weight for attraction

        public float repulsionDistance; // Define the distance for repulsion
        public float alignmentDistance; // Define the distance for alignment

        public float attractionWeight; // Define the weight for attraction
        public float repulsionWeight; // Define the weight for repulsion
        public float alignmentWeight; // Define the weight for alignment


        public BoundaryType boundaryType;
    }

    public struct DefaultBorgPhysicsTransformComponent : IComponentData
    {
        public PhysicsTransformComponent Settings;
    }

    public struct DefaultGoalPhysicsTransformComponent : IComponentData
    {
        public PhysicsTransformComponent Settings;
    }
}