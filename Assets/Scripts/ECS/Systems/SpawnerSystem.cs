using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Borgs
{
    [RequireMatchingQueriesForUpdate]
    [UpdateBefore(typeof(QuadTreeSystem))]
    [UpdateInGroup(typeof(CoreSimulationSystemGroup))]
    [CreateBefore(typeof(QuadTreeSystem))]
    [BurstCompile]
    public partial struct SpawnerSystem : ISystem
    {
        public EntityQuery spawnerDataQuery;
        public EntityQuery inputDataQuery;
        public EntityQuery boundaryAABBsInfoQuery;
        public EntityQuery quadtreeComponentQuery;
        public EntityQuery BorgsOutsideQuadTreesQuery;
        public EntityQuery GoalsOutsideQuadTreesQuery;
        public EntityQuery DefaultBorgPhysicsTransformComponent;
        public EntityQuery DefaultGoalPhysicsTransformComponent;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerDataComponent>();
            state.RequireForUpdate<InputData>();
            spawnerDataQuery = state.GetEntityQuery(ComponentType.ReadOnly<SpawnerDataComponent>());
            inputDataQuery = state.GetEntityQuery(ComponentType.ReadOnly<InputData>());
            boundaryAABBsInfoQuery = state.GetEntityQuery(ComponentType.ReadOnly<BoundaryAABBsInfo>());
            quadtreeComponentQuery = state.GetEntityQuery(ComponentType.ReadOnly<QuadtreeComponent>());
            BorgsOutsideQuadTreesQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), typeof(BorgTag), ComponentType.Exclude<InsertedIntoQuadtree>());
            GoalsOutsideQuadTreesQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), typeof(GoalTag), ComponentType.Exclude<InsertedIntoQuadtree>());


            DefaultBorgPhysicsTransformComponent = state.GetEntityQuery(ComponentType.ReadOnly<DefaultBorgPhysicsTransformComponent>());
            DefaultGoalPhysicsTransformComponent = state.GetEntityQuery(ComponentType.ReadOnly<DefaultGoalPhysicsTransformComponent>());



        }


        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }


        public void OnUpdate(ref SystemState state)
        {
            using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
            {

                // Retrieve the singleton data
                var spawnerDataEntity = spawnerDataQuery.GetSingletonEntity();
                var spawnerData = SystemAPI.GetComponent<SpawnerDataComponent>(spawnerDataEntity);



                if (!spawnerData.spawnerOn || spawnerData.BorgPrefabEntity == Entity.Null || spawnerData.GoalPrefabEntity == Entity.Null)
                {
                    return;
                }

                var commandBuffer = ecb.AsParallelWriter();
                double elapsedTime = SystemAPI.Time.ElapsedTime;
                var quadtreeComponentEntity = quadtreeComponentQuery.GetSingletonEntity();
                var quadTreeComponent = SystemAPI.GetComponent<QuadtreeComponent>(quadtreeComponentEntity);
                float3 spawnPosition = spawnerData.SpawnPosition;

                var defaultBorgPhysicsTransformComponent = SystemAPI.GetComponent<DefaultBorgPhysicsTransformComponent>(DefaultBorgPhysicsTransformComponent.GetSingletonEntity());
                var defaultGoalPhysicsTransformComponent = SystemAPI.GetComponent<DefaultGoalPhysicsTransformComponent>(DefaultGoalPhysicsTransformComponent.GetSingletonEntity());

                //if spawner is dirty , update the prefab enitty component to default physics transform component
                if (defaultBorgPhysicsTransformComponent.Settings.isDirty)
                {
                    if (spawnerData.BorgPrefabEntity != Entity.Null)
                    {
                        SystemAPI.SetComponent(spawnerData.BorgPrefabEntity, defaultBorgPhysicsTransformComponent.Settings);
                        defaultBorgPhysicsTransformComponent.Settings.isDirty = false;
                    }
                    SystemAPI.SetComponent(spawnerDataEntity, spawnerData);

                }
                if (defaultGoalPhysicsTransformComponent.Settings.isDirty)
                {
                    if (spawnerData.GoalPrefabEntity != Entity.Null)
                    {
                        SystemAPI.SetComponent(spawnerData.GoalPrefabEntity, defaultGoalPhysicsTransformComponent.Settings);
                        defaultGoalPhysicsTransformComponent.Settings.isDirty = false;
                    }
                    SystemAPI.SetComponent(spawnerDataEntity, spawnerData);

                }



                var inputData = inputDataQuery.GetSingleton<InputData>();

                if (spawnerData.Reset)
                {
                    spawnerData.Reset = false;
                    spawnerData.HasSpawnedInitial = false;
                    spawnerData.SpawnedBorgsCount = 0; // Reset counter
                    spawnerData.SpawnedGoalsCount = 0; // Reset counter
                    SystemAPI.SetComponent(spawnerDataEntity, spawnerData);
                }
                if (!spawnerData.HasSpawnedInitial)
                {
                    // Check if it's time to spawn initial entities based on the timer
                    if (elapsedTime >= spawnerData.Timer || spawnerData.spawnAtOnce)
                    {
                        int borgsToSpawn = math.min(spawnerData.initialBorgs - spawnerData.SpawnedBorgsCount, spawnerData.BorgSpawnBatchSize);
                        int goalsToSpawn = math.min(spawnerData.initialGoals - spawnerData.SpawnedGoalsCount, spawnerData.GoalSpawnBatchSize);

                        if (spawnerData.spawnAtOnce)
                        {
                            borgsToSpawn = spawnerData.initialBorgs;
                            goalsToSpawn = spawnerData.initialGoals;
                        }
                        if (borgsToSpawn > 0)
                        {

                            if (spawnerData.spawnAtOnce)
                            {
                                for (int i = 0; i < borgsToSpawn; i++)
                                {

                                    if (spawnerData.RandomizeSpawnPosition)
                                    {
                                        getSpawnPositionFromGlobals(spawnerData.BorgBoundaryType, out spawnPosition);
                                    }

                                    state.Dependency = ScheduleSpawnJob(commandBuffer, 1, spawnPosition, spawnerData.BorgPrefabEntity, "Borg", state.Dependency);
                                }
                            }
                            else
                            {

                                if (spawnerData.RandomizeSpawnPosition)
                                {
                                    getSpawnPositionFromGlobals(spawnerData.MoveableObjectBoundaryType, out spawnPosition);
                                }
                                state.Dependency = ScheduleSpawnJob(commandBuffer, borgsToSpawn, spawnPosition, spawnerData.BorgPrefabEntity, "Borg", state.Dependency);
                            }
                            spawnerData.SpawnedBorgsCount += borgsToSpawn;
                        }

                        if (goalsToSpawn > 0)
                        {
                            if (spawnerData.RandomizeSpawnPosition)
                            {
                                getSpawnPositionFromGlobals(spawnerData.MoveableObjectBoundaryType, out spawnPosition);
                            }
                            state.Dependency = ScheduleSpawnJob(commandBuffer, goalsToSpawn, spawnPosition, spawnerData.GoalPrefabEntity, "Goal", state.Dependency);
                            spawnerData.SpawnedGoalsCount += goalsToSpawn;
                        }

                        // Update the timer for the next spawn
                        spawnerData.Timer = (float)elapsedTime + spawnerData.SpawnInterval;

                        // Check if all initial entities have been spawned
                        if (spawnerData.SpawnedBorgsCount >= spawnerData.initialBorgs && spawnerData.SpawnedGoalsCount >= spawnerData.initialGoals)
                        {
                            spawnerData.HasSpawnedInitial = true;
                        }

                        SystemAPI.SetComponent(spawnerDataEntity, spawnerData);
                    }
                }

                // Check for user-initiated spawning
                if (inputData.BorgSpawnCommand.ShouldSpawn && inputData.BorgSpawnCommand.Amount > 0)
                {
                    state.Dependency = ScheduleSpawnJob(commandBuffer, inputData.BorgSpawnCommand.Amount, inputData.BorgSpawnCommand.SpawnPosition, spawnerData.BorgPrefabEntity, "Borg", state.Dependency);
                    inputData.BorgSpawnCommand.ShouldSpawn = false;

                }

                if (inputData.GoalSpawnCommand.ShouldSpawn && inputData.GoalSpawnCommand.Amount > 0)
                {
                    state.Dependency = ScheduleSpawnJob(commandBuffer, inputData.GoalSpawnCommand.Amount, inputData.GoalSpawnCommand.SpawnPosition, spawnerData.GoalPrefabEntity, "Goal", state.Dependency);
                    inputData.GoalSpawnCommand.ShouldSpawn = false;
                }

                state.Dependency.Complete();
                // Play back the EntityCommandBuffer's commands
                ecb.Playback(state.EntityManager);


                JobHandle ScheduleSpawnJob(EntityCommandBuffer.ParallelWriter ecb, int amount, float3 spawnPosition, Entity prefabEntity, string type, JobHandle inputDeps)
                {
                    var job = new SpawnJob
                    {
                        ECB = ecb,
                        SpawnPosition = spawnPosition,
                        PrefabEntity = prefabEntity,
                        physicsTransformsComponent = type == "Borg" ? defaultBorgPhysicsTransformComponent.Settings : defaultGoalPhysicsTransformComponent.Settings,
                        useTransformComponent = type == "Borg" ? spawnerData.useBorgPhysicsTransform : spawnerData.useGoalPhysicsTransform
                    };
                    var jobHandle = job.Schedule(amount, 128, inputDeps);
                    return jobHandle;
                }
            }
        }

        public void getSpawnPositionFromGlobals(BoundaryType boundaryType, out float3 spawnPosition)
        {
            spawnPosition = float3.zero;
            switch (boundaryType)
            {
                case BoundaryType.World:
                    Globals.worldBoundaries.GetRandomPositionFromCache(out spawnPosition);
                    break;
                case BoundaryType.PlayPen:
                    Globals.playPenBoundaries.GetRandomPositionFromCache(out spawnPosition);
                    break;
                default:
                    break;
            }
        }
        public struct InsertedIntoQuadtree : IComponentData { }

        [BurstCompile]
        struct SpawnJob : IJobParallelFor
        {
            public EntityCommandBuffer.ParallelWriter ECB;
            public float3 SpawnPosition;
            public Entity PrefabEntity;
            public PhysicsTransformComponent physicsTransformsComponent;
            public bool useTransformComponent;
            public void Execute(int index)
            {
                Entity entity = ECB.Instantiate(index, PrefabEntity);
                // Set LocalTransform with the spawn position and default rotation
                ECB.SetComponent(index, entity, new LocalTransform
                {
                    Position = SpawnPosition,
                    Rotation = quaternion.identity,
                    Scale = 1f // Assuming uniform scale
                });

                if (useTransformComponent)
                {
                    // Set the PhysicsTransformComponent with the default values
                    ECB.SetComponent(index, entity, physicsTransformsComponent);
                }


            }
        }
    }

}
