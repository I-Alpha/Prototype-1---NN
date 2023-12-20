
using Borgs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Borgs
{

    public class PhysicsTransformAuthoring : MonoBehaviour
    {
        public PhysicsSettingsConfig config;

        public float? speed;

        public float? acceleration;
        public float? angularAcceleration;
        public float? angularVelocity;

        public float? maxSpeed;
        public float? maxRotationSpeed;
        public float? maxAcceleration;
        public float? maxAngularAcceleration;
        public float? maxAngularVelocity;

        public float attractionDistance = 10.0f; // Define the distance for attraction
        public float repulsionDistance = 2.0f; // Define the distance for repulsion
        public float alignmentDistance = 5.0f; // Define the distance for alignment

        public float attractionWeight = 1.0f; // Define the weight for attraction
        public float repulsionWeight = 1.0f; // Define the weight for repulsion
        public float alignmentWeight = 1.0f; // Define the weight for alignment
        public class PhysicsTransformBaker : Baker<PhysicsTransformAuthoring>
        {
            public override void Bake(PhysicsTransformAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new PhysicsTransformComponent
                {
                    //check for config first from config editor window then from authoring.config then from authoring
                    speed = authoring.config != null ? authoring.config.speed : authoring.speed ?? 0,
                    acceleration = authoring.config != null ? authoring.config.acceleration : authoring.acceleration ?? 0,
                    angularAcceleration = authoring.config != null ? authoring.config.angularAcceleration : authoring.angularAcceleration ?? 0,
                    angularVelocity = authoring.config != null ? authoring.config.angularVelocity : authoring.angularVelocity ?? 0,
                    maxSpeed = authoring.config != null ? authoring.config.maxSpeed : authoring.maxSpeed ?? 0,
                    maxRotationSpeed = authoring.config != null ? authoring.config.maxRotationSpeed : authoring.maxRotationSpeed ?? 0,
                    maxAcceleration = authoring.config != null ? authoring.config.maxAcceleration : authoring.maxAcceleration ?? 0,
                    maxAngularAcceleration = authoring.config != null ? authoring.config.maxAngularAcceleration : authoring.maxAngularAcceleration ?? 0,
                    maxAngularVelocity = authoring.config != null ? authoring.config.maxAngularVelocity : authoring.maxAngularVelocity ?? 0,
                    borgAttractionDistance = authoring.config != null ? authoring.config.borgAttractionDistance : authoring.attractionDistance,
                    repulsionDistance = authoring.config != null ? authoring.config.repulsionDistance : authoring.repulsionDistance,
                    alignmentDistance = authoring.config != null ? authoring.config.alignmentDistance : authoring.alignmentDistance,
                    attractionWeight = authoring.config != null ? authoring.config.attractionWeight : authoring.attractionWeight,
                    repulsionWeight = authoring.config != null ? authoring.config.repulsionWeight : authoring.repulsionWeight,
                    alignmentWeight = authoring.config != null ? authoring.config.alignmentWeight : authoring.alignmentWeight,
                    boundaryType = authoring.config != null ? authoring.config.BoundaryType : BoundaryType.None,
                };
                AddComponent(entity, data);

                //create defautlt
                var defaultEntity = GetEntity(TransformUsageFlags.None);

            }
        }


    }

}