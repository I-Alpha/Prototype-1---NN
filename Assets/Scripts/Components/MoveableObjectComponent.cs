using Unity.Entities;

public struct MoveableObjectComponent : IComponentData
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
    public BoundaryType boundaryType ;
}

public enum BoundaryType
{
    World,
    PlayPen
}
public enum ObjectType
{
    Obstacle,
    Agent,
    Goal,
    // Add other types as needed
}