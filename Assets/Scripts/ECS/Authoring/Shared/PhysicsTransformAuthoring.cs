
using Borgs;
using Unity.Entities;
using UnityEngine;

public class PhysicsTransformAuthoring : MonoBehaviour
{
    public PhysicsTransformConfig config;

    public float? speed;
    public float? rotationSpeed;
    public float? acceleration;
    public float? angularAcceleration;
    public float? angularVelocity;

    public float? maxSpeed;
    public float? maxRotationSpeed;
    public float? maxAcceleration;
    public float? maxAngularAcceleration;
    public float? maxAngularVelocity;


    public class PhysicsTransformBaker : Baker<PhysicsTransformAuthoring>
    {
        public override void Bake(PhysicsTransformAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var data = new PhysicsTransformComponent
            {
                speed = authoring.speed ?? authoring.config.speed,
                rotationSpeed = authoring.rotationSpeed ?? authoring.config.rotationSpeed,
                acceleration = authoring.acceleration ?? authoring.config.acceleration,
                angularAcceleration = authoring.angularAcceleration ?? authoring.config.angularAcceleration,
                angularVelocity = authoring.angularVelocity ?? authoring.config.angularVelocity,
                maxSpeed = authoring.maxSpeed ?? authoring.config.maxSpeed,
                maxRotationSpeed = authoring.maxRotationSpeed ?? authoring.config.maxRotationSpeed,
                maxAcceleration = authoring.maxAcceleration ?? authoring.config.maxAcceleration,
                maxAngularAcceleration = authoring.maxAngularAcceleration ?? authoring.config.maxAngularAcceleration,
                maxAngularVelocity = authoring.maxAngularVelocity ?? authoring.config.maxAngularVelocity,
            };
            AddComponent(entity, data);
        }
    }


}
