using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using NativeTrees;
using Unity.Burst.Intrinsics;

namespace Borgs
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct UpdateBrainSystem : ISystem
    {
        private EntityQuery _quadtreeComponentQuery;
        private EntityQuery _borgEntitiesQuery;

        private Entity _fixedTimeSettingsEntity;
        private FixedTimeSettings _fixedTimeSettings;

        private EntityTypeHandle _entityTypeHandle;
        private ComponentTypeHandle<LocalTransform> _localTransformTypeHandle;
        private ComponentTypeHandle<PhysicsTransformComponent> _physicsTransformTypeHandle;
        private ComponentTypeHandle<BrainComponent> _brainComponentTypeHandle;

        private ComponentLookup<LocalTransform> _borgTransforms;
        private ComponentLookup<BrainComponent> _brainComponents;
        private float _accumulatedTime;


        public void OnCreate(ref SystemState state)
        {
            // Define the required components for the borg entities query
            _borgEntitiesQuery = state.GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
            ComponentType.ReadOnly<BorgTag>(),
            ComponentType.ReadWrite<BrainComponent>(),
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.ReadOnly<PhysicsTransformComponent>(),
            ComponentType.ReadWrite<BorgMovementCommandComponent>(),
            ComponentType.ReadOnly<InsertedIntoQuadTreeTag>()
                }
            });

            // Define the query for the QuadtreeComponent
            _quadtreeComponentQuery = state.GetEntityQuery(ComponentType.ReadOnly<QuadtreeComponent>());

            // Get the singleton entities
            _fixedTimeSettingsEntity = state.GetEntityQuery(ComponentType.ReadWrite<FixedTimeSettings>()).GetSingletonEntity();

            // Initialize component lookups and queues
            _borgTransforms = state.GetComponentLookup<LocalTransform>(true);

            _localTransformTypeHandle = state.GetComponentTypeHandle<LocalTransform>(true);
            _physicsTransformTypeHandle = state.GetComponentTypeHandle<PhysicsTransformComponent>(true);
            _brainComponentTypeHandle = state.GetComponentTypeHandle<BrainComponent>(false);
            _entityTypeHandle = state.GetEntityTypeHandle();
            _accumulatedTime = 0f;
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Early out if no entities are in the query
            if (_borgEntitiesQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            // Get the fixed time settings
            _fixedTimeSettings = SystemAPI.GetComponent<FixedTimeSettings>(_fixedTimeSettingsEntity);

            // Update logic based on fixed step settings
            if (_fixedTimeSettings.fixedStep)
            {
                _accumulatedTime += SystemAPI.Time.DeltaTime;
                while (_accumulatedTime >= _fixedTimeSettings.fixedStepTime)
                {
                    _accumulatedTime -= _fixedTimeSettings.fixedStepTime;
                    ExecuteUpdateLogic(ref state);
                }
            }
            else
            {
                ExecuteUpdateLogic(ref state);
            }
        }
        [BurstCompile]
        private void ExecuteUpdateLogic(ref SystemState state)
        {
            var quadtreeComponent = _quadtreeComponentQuery.GetSingleton<QuadtreeComponent>();
            if (!quadtreeComponent.BorgQuadtreeFilled)
            {
                return;
            }

            JobHandle findClosestHandle;
            JobHandle applySwarmHandle;

            // Update lookups
            _borgTransforms.Update(ref state);

            //update typehandles
            _localTransformTypeHandle.Update(ref state);
            _physicsTransformTypeHandle.Update(ref state);
            _brainComponentTypeHandle.Update(ref state);

            //update entity type handle
            _entityTypeHandle.Update(ref state);

            // Schedule the FindClosestEntityJobChunk
            var findClosestJobChunk = new FindClosestEntityJobChunk
            {
                QuadtreeComponent = quadtreeComponent,
                LocalTransformTypeHandle = _localTransformTypeHandle,
                allTransforms = _borgTransforms,
                PhysicsTransformTypeHandle = _physicsTransformTypeHandle,
                BrainComponentTypeHandle = _brainComponentTypeHandle,
                EntityTypeHandle = _entityTypeHandle,
            };

            findClosestHandle = findClosestJobChunk.ScheduleParallel(_borgEntitiesQuery, state.Dependency);
            state.Dependency = findClosestHandle;

            // Schedule the ApplySwarmBehaviorJobChunk
            var applySwarmJobChunk = new ApplySwarmBehaviorJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            };

            applySwarmHandle = applySwarmJobChunk.ScheduleParallel(_borgEntitiesQuery, state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(findClosestHandle, applySwarmHandle);
        }

        [BurstCompile]
        public struct FindClosestEntityJobChunk : IJobChunk
        {

            [ReadOnly] public QuadtreeComponent QuadtreeComponent;
            [ReadOnly] public ComponentTypeHandle<LocalTransform> LocalTransformTypeHandle;
            [ReadOnly] public ComponentTypeHandle<PhysicsTransformComponent> PhysicsTransformTypeHandle;
            [ReadOnly] public ComponentLookup<LocalTransform> allTransforms;
            [ReadOnly] public EntityTypeHandle EntityTypeHandle;
            public ComponentTypeHandle<BrainComponent> BrainComponentTypeHandle;

            [BurstCompile]
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var localTransforms = chunk.GetNativeArray(ref LocalTransformTypeHandle);
                var physicsTransforms = chunk.GetNativeArray(ref PhysicsTransformTypeHandle);
                var brainComponents = chunk.GetNativeArray(ref BrainComponentTypeHandle);
                var entities = chunk.GetNativeArray(EntityTypeHandle);

                // When calling FindClosestEntity
                using (var nearestQueue = new NativeQueue<Entity>(Allocator.Temp))
                {
                    using (var set = new NativeParallelHashSet<Entity>(1, Allocator.Temp))
                    {
                        for (int i = 0; i < chunk.Count; i++)
                        {
                            var transform = localTransforms[i];
                            var physicsTransform = physicsTransforms[i];
                            var entity = entities[i];
                            var position = transform.Position;
                            var closestEntityData = new ClosestEntitiesData();
                            var pos = position.xy;

                            var borgquadtree = QuadtreeComponent.BorgQuadtree;
                            var goalquadtree = QuadtreeComponent.GoalQuadtree;
                            FindClosestEntity(nearestQueue, set, entity.Index, ref borgquadtree, ref pos, physicsTransform.borgAttractionDistance, out closestEntityData.ClosestBorg, ref allTransforms);
                            FindClosestEntity(nearestQueue, set, entity.Index, ref goalquadtree, ref pos, physicsTransform.goalAttractionDistance, out closestEntityData.ClosestGoal, ref allTransforms);

                            ////find2 for borgs and goals
                            //FindClosestEntity2(entity.Index, QuadtreeComponent.BorgQuadtree, position.xy, physicsTransform.borgAttractionDistance, out closestEntityData.ClosestBorg, allTransforms);
                            //FindClosestEntity2(entity.Index, QuadtreeComponent.GoalQuadtree, position.xy, physicsTransform.goalAttractionDistance, out closestEntityData.ClosestGoal, allTransforms);

                            //FindClosestEntity(QuadtreeComponent.ObstacleQuadtree, position.xy, physicsTransform.borgAttractionDistance, out closestEntityData.ClosestObstacle);

                            brainComponents[i] = new BrainComponent { closestEntitiesData = closestEntityData };

                        }
                    }
                }
            }

            // Method to find the closest entity using a quadtree
            [BurstCompile]
            static void FindClosestEntity(NativeQueue<Entity> _nearestQueue, NativeParallelHashSet<Entity> set, int currentEntityIndex, ref NativeQuadtree<Entity> quadtree, ref float2 position, float radius, out EntityWithPosition entityWithPosition, ref ComponentLookup<LocalTransform> allTransforms)
            {
                entityWithPosition = new EntityWithPosition()
                {

                    Entity = Entity.Null,
                    Distance = float.MaxValue,
                    Direction = 0,
                    Position = float3.zero,
                };

                var nearestVisitors = new Nearest()
                {
                    nearest = _nearestQueue,
                    set = set,
                };

                quadtree.Nearest(position.xy, radius, ref nearestVisitors, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

                while (nearestVisitors.nearest.TryDequeue(out Entity closestEntity))
                {
                    var closestTransform = allTransforms[closestEntity];
                    var closestPosition = closestTransform.Position;
                    if (position.Equals(closestPosition))
                    {
                        continue;
                    }
                    var distance = math.distance(new(position, 0), closestPosition);
                    var res = position - new float2(closestPosition.x, closestPosition.y);
                    var direction = math.normalize(res);

                    entityWithPosition = new()
                    {
                        Entity = closestEntity,
                        Position = closestPosition,
                        Distance = distance,
                        Direction = direction,
                    };
                }

                _nearestQueue.Clear();
                set.Clear();
            }
        }

        [BurstCompile]
        void FindClosestEntity2(int currentEntityIndex, NativeQuadtree<Entity> quadtree, float2 position, float radius, out EntityWithPosition entityWithPosition, ComponentLookup<LocalTransform> allTransforms)
        {
            var visitor = new NearestExcludingCurrent { currentEntityIndex = currentEntityIndex };
            quadtree.Nearest(position, radius, ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));

            if (visitor.found)
            {
                var closestTransform = allTransforms[visitor.nearest];
                var closestPosition = closestTransform.Position;
                var distance = math.distance(new float3(position, 0), closestPosition);
                var res = position - new float2(closestPosition.x, closestPosition.y);
                var direction = math.normalize(res);

                entityWithPosition = new EntityWithPosition()
                {
                    Entity = visitor.nearest,
                    Position = closestPosition,
                    Distance = distance,
                    Direction = direction,
                };
            }
            else
            {
                entityWithPosition = new EntityWithPosition()
                {
                    Entity = Entity.Null,
                    Distance = float.MaxValue,
                    Position = float3.zero,
                    Direction = 0,
                };
            }
        }

        [BurstCompile]
        struct NearestExcludingCurrent : IQuadtreeNearestVisitor<Entity>
        {
            public int currentEntityIndex;
            public Entity nearest;
            public bool found;

            public bool OnVist(Entity obj)
            {
                if (obj.Index != currentEntityIndex)
                {
                    this.found = true;
                    this.nearest = obj;

                    return false; // stop iterating at first non-current hit
                }

                return true; // keep iterating if this is the current entity
            }
        }
        [BurstCompile]
        struct Nearest : IQuadtreeNearestVisitor<Entity>
        {
            public int instanceCount;
            public NativeQueue<Entity> nearest;
            public NativeParallelHashSet<Entity> set;
            public bool OnVist(Entity obj)
            {
                if (set.Add(obj))
                {
                    nearest.Enqueue(obj);
                    instanceCount++;
                }

                return instanceCount < 2;
            }

        }
    }

    [BurstCompile]
    public partial struct ApplySwarmBehaviorJob : IJobEntity
    {
        public float deltaTime;

        [BurstCompile]
        public void Execute(ref BorgMovementCommandComponent borgMovementCommandComponent, ref BrainComponent brainComponent, in PhysicsTransformComponent physicsTransformComponent, in LocalTransform localTransform, in BorgTag borgTag)
        {
            float2 forward2D = new float2(0, 1); // Assuming forward is (0, 1) in local space

            // Reset values
            borgMovementCommandComponent.Thrust = 0;
            borgMovementCommandComponent.Rotation = 0;

            // Process closest Borg
            ProcessClosestEntity(ref borgMovementCommandComponent, brainComponent.closestEntitiesData.ClosestBorg, forward2D);

            // Process closest Goal
            ProcessClosestEntity(ref borgMovementCommandComponent, brainComponent.closestEntitiesData.ClosestGoal, forward2D);
        }

        [BurstCompile]
        private void ProcessClosestEntity(ref BorgMovementCommandComponent borgMovementCommandComponent, EntityWithPosition closestEntity, float2 forward2D)
        {
            if (closestEntity.Entity != Entity.Null)
            {
                // Compute the angle between current forward direction and target direction
                float angle = computeRelativeSignedAngleBetweenDirections(closestEntity.Direction, forward2D);

                // Clamp the rotation to prevent over-rotation
                angle = math.clamp(angle, -180, 180);

                // Set rotation and thrust
                borgMovementCommandComponent.Rotation += angle;
                borgMovementCommandComponent.Thrust = 1;
            }
        }

        // angle in degrees
        [BurstCompile]
        float computeAngleBetweenPoints(Vector2 a, Vector2 b)
        {
            var diff = b - a;
            return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        }

        // angle in degrees; directions must be of unit length (normalized)
        [BurstCompile]
        float computeRelativeAngleBetweenDirections(Vector2 dir1, Vector2 dir2)
        {
            return Mathf.Atan2(Vector3.Cross(dir1, dir2).magnitude, Vector3.Dot(dir1, dir2)) * Mathf.Rad2Deg;
        }

        // angle in degrees; directions must be of unit length (normalized)
        [BurstCompile]
        float computeRelativeSignedAngleBetweenDirections(Vector2 dir1, Vector2 dir2)
        {
            return Mathf.Atan2(Vector3.Dot(Vector3.Cross(dir1, dir2), Vector3.back), Vector3.Dot(dir1, dir2)) * Mathf.Rad2Deg;
        }

        private void ToFloat3(ref float2 value, out float3 result)
        {
            result = new float3(value.x, value.y, 0);
        }

        private void ToFloat2(float3 value, out float2 result)
        {
            result = new float2(value.x, value.y);
        }
    }
}
