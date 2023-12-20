//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Transforms;
//using Unity.Mathematics;
//using Unity.Jobs;
//using UnityEngine;
//using NativeTrees;
//using System.Linq;

//namespace Borgs
//{
//    [BurstCompile]
//    [RequireMatchingQueriesForUpdate]
//    [UpdateInGroup(typeof(SimulationSystemGroup))]
//    public partial struct UpdateBrainSystem : ISystem
//    {
//        private EntityQuery _quadtreeComponentQuery;
//        private EntityQuery _borgEntitiesQuery;

//        private Entity _fixedTimeSettingsEntity;
//        private FixedTimeSettings _fixedTimeSettings;

//        private ComponentLookup<LocalTransform> _borgTransforms;
//        private ComponentLookup<BrainComponent> _brainComponents;
//        private ComponentLookup<PhysicsTransformComponent> _physicsTransformComponents;

//        private float _accumulatedTime;


//        public void OnCreate(ref SystemState state)
//        {
//            // Define the required components for the borg entities query
//            _borgEntitiesQuery = state.GetEntityQuery(new EntityQueryDesc
//            {
//                All = new ComponentType[]
//                {
//            ComponentType.ReadOnly<BorgTag>(),
//            ComponentType.ReadWrite<BrainComponent>(),
//            ComponentType.ReadOnly<LocalTransform>(),
//            ComponentType.ReadOnly<PhysicsTransformComponent>(),
//            ComponentType.ReadWrite<BorgMovementCommandComponent>(),
//            ComponentType.ReadOnly<InsertedIntoQuadTreeTag>()
//                }
//            });

//            // Define the query for the QuadtreeComponent
//            _quadtreeComponentQuery = state.GetEntityQuery(ComponentType.ReadOnly<QuadtreeComponent>());

//            // Get the singleton entities
//            _fixedTimeSettingsEntity = state.GetEntityQuery(ComponentType.ReadWrite<FixedTimeSettings>()).GetSingletonEntity();

//            // Initialize component lookups and queues
//            _borgTransforms = state.GetComponentLookup<LocalTransform>(true);
//            _brainComponents = state.GetComponentLookup<BrainComponent>(false);
//            _physicsTransformComponents = state.GetComponentLookup<PhysicsTransformComponent>(true);

//            _accumulatedTime = 0f;
//        }


//        [BurstCompile]
//        public void OnUpdate(ref SystemState state)
//        {
//            // Early out if no entities are in the query
//            if (_borgEntitiesQuery.IsEmptyIgnoreFilter)
//            {
//                return;
//            }

//            // Get the fixed time settings
//            _fixedTimeSettings = SystemAPI.GetComponent<FixedTimeSettings>(_fixedTimeSettingsEntity);

//            // Update logic based on fixed step settings
//            if (_fixedTimeSettings.fixedStep)
//            {
//                _accumulatedTime += SystemAPI.Time.DeltaTime;
//                while (_accumulatedTime >= _fixedTimeSettings.fixedStepTime)
//                {
//                    _accumulatedTime -= _fixedTimeSettings.fixedStepTime;
//                    ExecuteUpdateLogic(ref state);
//                }
//            }
//            else
//            {
//                ExecuteUpdateLogic(ref state);
//            }
//        }


//        [BurstCompile]
//        private void ExecuteUpdateLogic(ref SystemState state)
//        {
//            var quadtreeComponent = _quadtreeComponentQuery.GetSingleton<QuadtreeComponent>();
//            if (!quadtreeComponent.BorgQuadtreeFilled)
//            {
//                // Quadtree not ready, skip this update
//                return;
//            }

//            JobHandle findClosestHandle;
//            JobHandle applySwarmHandle;

//            using (var borgEntities = _borgEntitiesQuery.ToEntityArray(Allocator.TempJob))
//            {


//                if (borgEntities.Length == 0)
//                {
//                    return;
//                }

//                //update lookups
//                _borgTransforms.Update(ref state);
//                _brainComponents.Update(ref state);
//                _physicsTransformComponents.Update(ref state);


//                // Create NativeArray to store closest entities results
//                using (var closestEntitiesResults = new NativeArray<ClosestEntitiesData>(borgEntities.Length, Allocator.TempJob))
//                {

//                    // Schedule the FindClosestEntityJob
//                    var findClosestJob = new FindClosestEntityJob
//                    {
//                        entities = borgEntities,
//                        quadtreeComponent = quadtreeComponent, // Pass the quadtreeComponent to the job
//                        closestEntities = closestEntitiesResults,
//                        allTransforms = _borgTransforms,
//                        allPhysicsTransforms = _physicsTransformComponents,
//                        nearestQueue = new NativeQueue<Entity>().AsParallelWriter(),
//                    };

//                    findClosestHandle = findClosestJob.Schedule(borgEntities.Length, 256, state.Dependency);
//                    state.Dependency = findClosestHandle;
//                    state.CompleteDependency();
//                    for (int i = 0; i < borgEntities.Length; i++)
//                    {
//                        var entity = borgEntities[i];
//                        _brainComponents[entity] = new BrainComponent(closestEntitiesResults[i]);
//                    }

//                }
//            }

//            // Schedule the ApplySwarmBehaviorJob, ensure it depends on the completion of FindClosestEntityJob
//            var applySwarmJob = new ApplySwarmBehaviorJob()
//            {
//                deltaTime = SystemAPI.Time.DeltaTime,
//            };

//            state.Dependency = applySwarmJob.ScheduleParallel(_borgEntitiesQuery, state.Dependency);
//            state.Dependency = JobHandle.CombineDependencies(findClosestHandle, state.Dependency);

//        }

//        [BurstCompile]
//        public struct FindClosestEntityJob : IJobParallelFor
//        {
//            [ReadOnly] public NativeArray<Entity> entities;
//            [ReadOnly] public QuadtreeComponent quadtreeComponent;
//            [ReadOnly] public ComponentLookup<LocalTransform> allTransforms;
//            [ReadOnly] public ComponentLookup<PhysicsTransformComponent> allPhysicsTransforms;
//            public NativeArray<ClosestEntitiesData> closestEntities;
//            public NativeQueue<Entity>.ParallelWriter nearestQueue;

//            [BurstCompile]
//            public void Execute(int index)
//            {
//                var entity = entities[index];
//                var transform = allTransforms[entity];
//                var physicsTransform = allPhysicsTransforms[entity];

//                var position = transform.Position;

//                var closestEntityData = new ClosestEntitiesData();
//                // Assuming you have methods to find the closest entity for each type
//                FindClosestEntity(quadtreeComponent.BorgQuadtree, position.xy, physicsTransform.borgAttractionDistance, out closestEntityData.ClosestBorg);
//                FindClosestEntity(quadtreeComponent.GoalQuadtree, position.xy, physicsTransform.goalAttractionDistance, out closestEntityData.ClosestGoal);
//                //FindClosestEntity(quadtreeComponent.ObstacleQuadtree, position.xy, physicsTransform.borgAttractionDistance, out closestEntityData.ClosestObstacle);


//                closestEntities[index] = closestEntityData;

//            }

//            // Method to find the closest entity using a quadtree
//            [BurstCompile]
//            private void FindClosestEntity(NativeQuadtree<Entity> quadtree, float2 position, float radius, out EntityWithPosition entityWithPosition)
//            {
//                entityWithPosition = new EntityWithPosition()
//                {
//                    Entity = Entity.Null,
//                    Distance = float.MaxValue,
//                    Direction = 0,
//                    Position = float3.zero,
//                };

//                using (var set = new NativeParallelHashSet<Entity>(1, Allocator.Temp))
//                {

//                    var nearestVisitors = new Nearest()
//                    {
//                        nearest = nearestQueue,
//                        set = set,
//                    };


//                    quadtree.Nearest(position.xy, radius, ref nearestVisitors, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<Entity>));


//                    //loop through the queue and find the closest entity
//                    using (NativeArray<Entity> closestEntity = set.ToNativeArray(Allocator.Temp))
//                    {
//                        for (int i = 0; i < closestEntity.Length; i++)
//                        {
//                            var closestTransform = allTransforms[closestEntity[i]];
//                            var closestPosition = closestTransform.Position;

//                            var distance = math.distance(new(position, 0), closestPosition);

//                            if (distance > 0 && distance < entityWithPosition.Distance)
//                            {
//                                entityWithPosition = new()
//                                {
//                                    Entity = closestEntity[i],
//                                    Position = closestPosition,
//                                    Distance = distance,
//                                };
//                            }
//                        }
//                    }
//                }
//            }

//            [BurstCompile]
//            struct Nearest : IQuadtreeNearestVisitor<Entity>
//            {
//                public NativeQueue<Entity>.ParallelWriter nearest;
//                public NativeParallelHashSet<Entity> set;
//                public int instanceCount;
//                public bool OnVist(Entity obj)
//                {
//                    if (set.Add(obj))
//                    {
//                        nearest.Enqueue(obj);
//                        instanceCount++;
//                    }

//                    return instanceCount < 2;
//                }
//            }
//        }

//        [BurstCompile]
//        public partial struct ApplySwarmBehaviorJob : IJobEntity
//        {
//            public float deltaTime;

//            [BurstCompile]
//            public void Execute(ref BorgMovementCommandComponent borgMovementCommandComponent, ref BrainComponent brainComponent, in PhysicsTransformComponent physicsTransformComponent, in LocalTransform localTransform, in BorgTag borgTag)
//            {
//                float3 currentPosition = localTransform.Position;
//                float2 forward2D = new float2(0, 1); // Assuming forward is (0, 1) in local space

//                // Reset values
//                borgMovementCommandComponent.Thrust = 0;
//                borgMovementCommandComponent.Rotation = 0;

//                // Process closest Borg
//                ProcessClosestEntity(ref borgMovementCommandComponent, brainComponent.closestEntitiesData.ClosestBorg, currentPosition, forward2D);

//                // Process closest Goal
//                ProcessClosestEntity(ref borgMovementCommandComponent, brainComponent.closestEntitiesData.ClosestGoal, currentPosition, forward2D);
//            }

//            [BurstCompile]
//            private void ProcessClosestEntity(ref BorgMovementCommandComponent borgMovementCommandComponent, EntityWithPosition closestEntity, float3 currentPosition, float2 forward2D)
//            {
//                if (closestEntity.Entity != Entity.Null)
//                {
//                    var closestEntityPosition = closestEntity.Position;

//                    // Calculate the direction vector and angle
//                    float3 relativePosition = closestEntityPosition - currentPosition;
//                    float2 direction2D = math.normalize(new float2(relativePosition.x, relativePosition.y));

//                    // Compute the angle between current forward direction and target direction
//                    float angle = computeRelativeSignedAngleBetweenDirections(direction2D, forward2D);

//                    // Clamp the rotation to prevent over-rotation
//                    angle = math.clamp(angle, -180, 180);

//                    // Set rotation and thrust
//                    borgMovementCommandComponent.Rotation += angle;
//                    borgMovementCommandComponent.Thrust = 1;
//                }
//            }

//            // angle in degrees
//            [BurstCompile]
//            float computeAngleBetweenPoints(Vector2 a, Vector2 b)
//            {
//                var diff = b - a;
//                return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
//            }

//            // angle in degrees; directions must be of unit length (normalized)
//            [BurstCompile]
//            float computeRelativeAngleBetweenDirections(Vector2 dir1, Vector2 dir2)
//            {
//                return Mathf.Atan2(Vector3.Cross(dir1, dir2).magnitude, Vector3.Dot(dir1, dir2)) * Mathf.Rad2Deg;
//            }

//            // angle in degrees; directions must be of unit length (normalized)
//            [BurstCompile]
//            float computeRelativeSignedAngleBetweenDirections(Vector2 dir1, Vector2 dir2)
//            {
//                return Mathf.Atan2(Vector3.Dot(Vector3.Cross(dir1, dir2), Vector3.back), Vector3.Dot(dir1, dir2)) * Mathf.Rad2Deg;
//            }

//            private void ToFloat3(ref float2 value, out float3 result)
//            {
//                result = new float3(value.x, value.y, 0);
//            }

//            private void ToFloat2(float3 value, out float2 result)
//            {
//                result = new float2(value.x, value.y);
//            }
//        }
//    }
//}
