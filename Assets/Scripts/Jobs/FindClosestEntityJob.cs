using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct EntityWithPosition
{
    public Entity Entity;
    public float Distance;
    public float Direction;
    public float3 Position;
    public ObjectType type;
}

public struct ClosestEntitiesData
{
    public int index;
    public EntityWithPosition ClosestAgent;
    public EntityWithPosition ClosestGoal;
    public EntityWithPosition ClosestObstacle;
}

[BurstCompile]
public struct FindClosestEntityJob : IJobChunk
{
    public float SearchRadius; // New field for search radius 

    [ReadOnly] public ComponentTypeHandle<LocalTransform> LocalTransformTypeHandle;
    [ReadOnly] public ComponentTypeHandle<MoveableObjectComponent> MoveableObjectDataTypeHandle;
    [ReadOnly] public EntityTypeHandle EntityTypeHandle;


    [ReadOnly] public NativeArray<ClosestEntitiesData> ReadBuffer; // Buffer for reading
    public NativeArray<ClosestEntitiesData> WriteBuffer; // Buffer for writing


    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var chunkLocalTransforms = chunk.GetNativeArray(ref LocalTransformTypeHandle);
        var chunkMoveableObjectData = chunk.GetNativeArray(ref MoveableObjectDataTypeHandle);
        var chunkEntities = chunk.GetNativeArray(EntityTypeHandle);

        for (int i = 0; i < chunk.Count; i++)
        { 
            int index = unfilteredChunkIndex * chunk.Capacity + i;
            var writeData = new ClosestEntitiesData
            {
                index = index,
                ClosestAgent = InitializeEntityWithPosition(),
                ClosestGoal = InitializeEntityWithPosition(),
                ClosestObstacle = InitializeEntityWithPosition()
            };

            for (int j = 0; j < chunk.Count; j++)
            {
                if (i != j)
                {
                    float distance = math.distance(chunkLocalTransforms[i].Position.xy, chunkLocalTransforms[j].Position.xy);
                    if (distance <= SearchRadius)
                    {
                        var targetType = chunkMoveableObjectData[j].type;
                        UpdateClosestEntity(ref writeData, targetType, chunkEntities[j], distance, chunkLocalTransforms[j].Position);
                    }
                }
            }

            WriteBuffer[index] = writeData;
        }
    }
    private EntityWithPosition InitializeEntityWithPosition()
    {
        return new EntityWithPosition
        {
            Entity = Entity.Null,
            Distance = float.MaxValue,
            Position = float3.zero
        };
    }
    private void UpdateClosestEntity(ref ClosestEntitiesData data, ObjectType targetType, Entity targetEntity, float distance, float3 position)
    {
        var entityWithPosition = new EntityWithPosition { Entity = targetEntity, Distance = distance, Position = position };

        switch (targetType)
        {
            case ObjectType.Agent:
                if (distance < data.ClosestAgent.Distance)
                    data.ClosestAgent = entityWithPosition;
                break;
            case ObjectType.Goal:
                if (distance < data.ClosestGoal.Distance)
                    data.ClosestGoal = entityWithPosition;
                break;
            case ObjectType.Obstacle:
                if (distance < data.ClosestObstacle.Distance)
                    data.ClosestObstacle = entityWithPosition;
                break;
        }
    }
}



