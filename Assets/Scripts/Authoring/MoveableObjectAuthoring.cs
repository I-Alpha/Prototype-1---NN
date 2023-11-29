
using Unity.Entities;
using UnityEngine;

public class MoveableObjectAuthoring : MonoBehaviour
{
    public MoveableObjectConfig config;

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

    public ObjectType type;
    public BoundaryType boundaryType;

    public class MoveableObjectBaker : Baker<MoveableObjectAuthoring>
    {
        public override void Bake (MoveableObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var data = new MoveableObjectComponent
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
                type = authoring.config.type,
                boundaryType =  authoring.config.boundaryType     
            };
            AddComponent(entity, data);
        }
    }


}
