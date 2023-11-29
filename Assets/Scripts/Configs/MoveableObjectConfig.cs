using UnityEngine;

[CreateAssetMenu(fileName = "MoveableObjectConfig", menuName = "Configurations/MoveableObjectConfig")]
public class MoveableObjectConfig : ScriptableObject
{
    public bool IsDirty { get; set; }
    public float speed;
    public float rotationSpeed;
    public float acceleration;
    public float angularAcceleration;
    public float angularVelocity;

    public float maxSpeed = 5;
    public float maxRotationSpeed = 5;
    public float maxAcceleration = 5;
    public float maxAngularAcceleration = 5;
    public float maxAngularVelocity = 5;

    public ObjectType type;
    public BoundaryType boundaryType;
     
    private void OnValidate()
    {
        IsDirty = true;
    }
}
