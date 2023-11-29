using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public partial class EntityNavigationSystem : SystemBase
{
    private EntityQuery _query;
    private NativeArray<ClosestEntitiesData> _bufferA;
    private NativeArray<ClosestEntitiesData> _bufferB;
    private bool _useBufferA;

    protected override void OnCreate()
    {
        _query = GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<MoveableObjectComponent>());
        int entityCount = _query.CalculateEntityCount();
        _bufferA = new NativeArray<ClosestEntitiesData>(entityCount, Allocator.TempJob);
        _bufferB = new NativeArray<ClosestEntitiesData>(entityCount, Allocator.TempJob);
        _useBufferA = true;
    }

    protected override void OnUpdate()
    {
        int entityCount = _query.CalculateEntityCount();
        if (entityCount > 0)
        {
            HandleFindClosestEntityJob(entityCount);
        }
    }
    private void HandleFindClosestEntityJob(int entityCount)
    {
        resizeClosestEntitiesDataBuffers(entityCount);

        NativeArray<ClosestEntitiesData> readBuffer = _useBufferA ? _bufferA : _bufferB;
        NativeArray<ClosestEntitiesData> writeBuffer = _useBufferA ? _bufferB : _bufferA;

        var findClosestEntityJob = new FindClosestEntityJob
        {
            SearchRadius = 10f,
            LocalTransformTypeHandle = GetComponentTypeHandle<LocalTransform>(true),
            MoveableObjectDataTypeHandle = GetComponentTypeHandle<MoveableObjectComponent>(true),
            EntityTypeHandle = GetEntityTypeHandle(),
            ReadBuffer = readBuffer,
            WriteBuffer = writeBuffer
        };

        Dependency = findClosestEntityJob.ScheduleParallel(_query, Dependency); 

        ProcessClosestEntitiesData(writeBuffer);

        // Swap buffers for the next frame
        _useBufferA = !_useBufferA;
    }

    private void resizeClosestEntitiesDataBuffers(int entityCount)
    {
        // Resize buffers if necessary
        if (_bufferA.Length != entityCount)
        {
            if (_bufferA.IsCreated)
                _bufferA.Dispose();
            _bufferA = new NativeArray<ClosestEntitiesData>(entityCount, Allocator.TempJob);
        }

        if (_bufferB.Length != entityCount)
        {
            if (_bufferB.IsCreated)
                _bufferB.Dispose();
            _bufferB = new NativeArray<ClosestEntitiesData>(entityCount, Allocator.TempJob);
        }
    }

    protected override void OnDestroy()
    {
        if (_bufferA.IsCreated) _bufferA.Dispose();
        if (_bufferB.IsCreated) _bufferB.Dispose();
    }
     
    private void ProcessClosestEntitiesData(NativeArray<ClosestEntitiesData> closestEntitiesData)
    {
        Entities.ForEach((ref BrainComponent agentTarget, in Entity entity) =>
        {
            // Assuming the entity index is properly correlated with the closestEntitiesData array
            int index = entity.Index;
            if (index < closestEntitiesData.Length)
            {
                var closestData = closestEntitiesData[index];

                // Update the agent's target based on closest goal or obstacle
                if (closestData.ClosestGoal.Entity != Entity.Null)
                {
                    agentTarget.closestEntitiesData.ClosestGoal = closestData.ClosestGoal;
                }
                if (closestData.ClosestAgent.Entity != Entity.Null)
                {
                    agentTarget.closestEntitiesData.ClosestAgent = closestData.ClosestGoal;
                }
                if (closestData.ClosestObstacle.Entity != Entity.Null)
                {
                    agentTarget.closestEntitiesData.ClosestObstacle = closestData.ClosestObstacle;
                }
            }
        }).WithoutBurst().Run(); // Using WithoutBurst().Run() for accessing EntityManager safely
    }
}

