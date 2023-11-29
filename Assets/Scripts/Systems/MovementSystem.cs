using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovementSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = (float)SystemAPI.Time.DeltaTime;

        foreach (var (brain, mvData, transform) in SystemAPI.Query<RefRW<BrainComponent>, RefRW<MoveableObjectComponent>, RefRW<LocalTransform>>())
        {
            BaseBoundaries2D boundary2d = GetBoundaries(mvData.ValueRO.boundaryType);
            // Apply acceleration and velocity
            MovementUtilities.ApplyAcceleration(ref mvData.ValueRW, deltaTime);
            MovementUtilities.ApplyAngularAcceleration(ref mvData.ValueRW, deltaTime);

            // Update position with clamping
            var newPosition = MovementUtilities.UpdatePosition(transform.ValueRW.Position, transform.ValueRW.Up().xy, mvData.ValueRW.speed, deltaTime);

            newPosition = new float3(boundary2d.ClampPosition(newPosition.xy), newPosition.z);

            transform.ValueRW.Position = newPosition;

            // Update rotation
            transform.ValueRW.Rotation = MovementUtilities.UpdateRotation(transform.ValueRW.Rotation, mvData.ValueRW.angularVelocity, deltaTime);
        }

    }
    public BaseBoundaries2D GetBoundaries(BoundaryType boundaryType)
    {
        switch (boundaryType)
        {
            case BoundaryType.World:
                return Globals.worldBoundaries;
            case BoundaryType.PlayPen:
                return Globals.playPenBoundaries;
            default:
                return Globals.playPenBoundaries;
        }
    }

}


public static class MovementUtilities
{
    public static void ApplyAcceleration(ref MoveableObjectComponent mvData, float deltaTime)
    {
        mvData.acceleration = math.clamp(mvData.acceleration, -mvData.maxAcceleration, mvData.maxAcceleration);
        mvData.speed += mvData.acceleration * deltaTime;
        mvData.speed = math.clamp(mvData.speed, -mvData.maxSpeed, mvData.maxSpeed);
    }

    public static void ApplyAngularAcceleration(ref MoveableObjectComponent mvData, float deltaTime)
    {
        mvData.angularAcceleration = math.clamp(mvData.angularAcceleration, -mvData.maxAngularAcceleration, mvData.maxAngularAcceleration);
        mvData.angularVelocity += mvData.angularAcceleration * deltaTime;
        mvData.angularVelocity = math.clamp(mvData.angularVelocity, -mvData.maxAngularVelocity, mvData.maxAngularVelocity);
    }

    public static float3 UpdatePosition(float3 currentPosition, float2 direction, float speed, float deltaTime)
    {
        var newPosition2D = currentPosition.xy + direction * speed * deltaTime;
        return new float3(newPosition2D, currentPosition.z);
    }

    public static quaternion UpdateRotation(quaternion currentRotation, float angularVelocity, float deltaTime)
    {
        return math.mul(currentRotation, quaternion.EulerXYZ(new float3(0, 0, angularVelocity * deltaTime)));
    }
}