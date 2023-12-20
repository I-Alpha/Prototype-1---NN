using NativeTrees;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Borgs
{
    [RequireMatchingQueriesForUpdate]
    [CreateAfter(typeof(UpdateBrainSystem))]
    [UpdateAfter(typeof(UpdateBrainSystem))]
    [UpdateInGroup(typeof(CoreSimulationSystemGroup))]
    [BurstCompile]
    public partial struct PhysicsTransformSystem : ISystem
    {
        EntityQuery m_BorgQuery;
        EntityQuery boundaryAABBsInfo;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BoundaryAABBsInfo>();

            m_BorgQuery = state.GetEntityQuery(ComponentType.ReadWrite<PhysicsTransformComponent>(), ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<BorgMovementCommandComponent>(), ComponentType.ReadOnly<BorgTag>());
            boundaryAABBsInfo = state.GetEntityQuery(ComponentType.ReadOnly<BoundaryAABBsInfo>());
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var boundaryAABBsInfoComponent = SystemAPI.GetComponent<BoundaryAABBsInfo>(boundaryAABBsInfo.GetSingletonEntity());
            // Schedule separate jobs for different boundary types
            var movementJobPlayPen = new MovementJob
            {
                deltaTime = deltaTime,
                playPenAABB = boundaryAABBsInfoComponent.playpenAABB,
                worldAABB = boundaryAABBsInfoComponent.worldAABB,
            };

            state.Dependency = movementJobPlayPen.ScheduleParallel(m_BorgQuery, state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct MovementJob : IJobEntity
    {
        public float deltaTime;
        public AABB2D playPenAABB;
        public AABB2D worldAABB;

        [BurstCompile]
        public void Execute(ref PhysicsTransformComponent physicsTransformComponent, ref LocalTransform localTransform, in BorgMovementCommandComponent borgMovementCommandComponent, in BorgTag borgTag)
        {
            // Apply the rotation
            quaternion targetRotation = quaternion.RotateZ(math.radians(borgMovementCommandComponent.Rotation));
            localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, deltaTime * physicsTransformComponent.rotationSpeed);

            // Apply thrust
            var forwardDirection = math.mul(localTransform.Rotation, new float3(0, 1, 0));
            var movement = forwardDirection * borgMovementCommandComponent.Thrust * deltaTime * physicsTransformComponent.speed;
            localTransform.Position += movement;

            // Enforce boundaries
            localTransform.Position = math.clamp(localTransform.Position, new float3(worldAABB.min.x, worldAABB.min.y, 0), new float3(worldAABB.max.x, worldAABB.max.y, 0));
        }
    }
}

