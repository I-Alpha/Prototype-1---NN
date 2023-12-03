using Unity.Entities;

public struct PhysicsTransformComponent : IComponentData
{
    public float speed;
    public float rotationSpeed;
    public float acceleration;
    public float angularAcceleration;
    public float angularVelocity;

    public float maxSpeed;
    public float maxRotationSpeed;
    public float maxAcceleration;
    public float maxAngularAcceleration;
    public float maxAngularVelocity;

    public ObjectType type;
    public BoundaryType boundaryType;
}

public enum BoundaryType
{
    World,
    PlayPen,
    None
}
public enum ObjectType
{
    Obstacle,
    Borg,
    Goal,
    // Add other types as needed
}