using UnityEngine;

namespace Borgs
{
    [CreateAssetMenu(fileName = "PhysicsSettingsConfig", menuName = "Configurations/PhysicsSettingsConfig")]
    public class PhysicsSettingsConfig : ScriptableObject, IConfig
    {
        public float speed = 1;
        public float acceleration;
        public float rotationSpeed = 1;

        public float angularAcceleration;
        public float angularVelocity;

        public float maxSpeed = 5;
        public float maxRotationSpeed = 5;
        public float maxAcceleration = 5;
        public float maxAngularAcceleration = 5;
        public float maxAngularVelocity = 5;



        public float borgAttractionDistance = 10.0f; // Define the distance for attraction
        public float goalAttractionDistance = 10.0f; // Define the distance for attraction
        public float attractionMultiplier = 1.0f; // Define the weight for attraction

        public float repulsionDistance = 2.0f; // Define the distance for repulsion
        public float alignmentDistance = 5.0f; // Define the distance for alignment

        public float attractionWeight = 1.0f; // Define the weight for attraction
        public float repulsionWeight = 1.0f; // Define the weight for repulsion
        public float alignmentWeight = 1.0f; // Define the weight for alignment

        public bool usePhysicsTransform = true;

        public BoundaryType BoundaryType = BoundaryType.PlayPen;


        public bool IsDirty { get; set; }

        private void OnValidate()
        {
            IsDirty = true;
        }

        public void UpdateConfig()
        {
            // Implement the logic to update the configuration settings
            // Example: You might save these settings to a file or update other game components
        }
    }
}