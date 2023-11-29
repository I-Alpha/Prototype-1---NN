using System.Windows.Input;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.EventSystems.EventTrigger;

public partial struct SpawnerSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        // Get the command buffer
        var ecb = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        // Process initial spawning
        SpawnInitialEntities(ref state, ecb);

        // Process input-based spawning
        var inputDataEntity = SystemAPI.GetSingletonEntity<InputData>();
        var inputData = SystemAPI.GetComponent<InputData>(inputDataEntity);
        ProcessInputCommands(ref state, inputData, ecb);
    }

    private void ProcessInputCommands(ref SystemState state, InputData inputData, EntityCommandBuffer ecb)
    {
        var spawnerDataEntity = SystemAPI.GetSingletonEntity<SpawnerComponent>();
        var spawnerData = SystemAPI.GetComponent<SpawnerComponent>(spawnerDataEntity);

        // Spawn agents based on input
        if (inputData.AgentSpawnCommand.ShouldSpawn)
        {
            SpawnEntities(inputData.AgentSpawnCommand, spawnerData.AgentPrefabEntity, ecb);
            inputData.AgentSpawnCommand.ShouldSpawn = false;
        }

        // Spawn goals based on input
        if (inputData.GoalSpawnCommand.ShouldSpawn)
        {
            SpawnEntities(inputData.GoalSpawnCommand, spawnerData.GoalPrefabEntity, ecb);
            inputData.GoalSpawnCommand.ShouldSpawn = false;
        }
    }
    private void SpawnEntities(SpawnCommand command, Entity prefabEntity, EntityCommandBuffer ecb)
    {
        for (int i = 0; i < command.Amount; i++)
        {
            Entity entity = ecb.Instantiate(prefabEntity);

            //add brain component for holding sensor data
            ecb.AddComponent(entity, new BrainComponent
            {
                closestEntitiesData = new ClosestEntitiesData
                {
                }
            });

            LocalTransform localTransform = new LocalTransform
            {
                Position = command.SpawnPosition,
                Rotation = Quaternion.identity,
                Scale = 1.0f
            };
            ecb.SetComponent(entity, localTransform);

        }
    }
    private void SpawnInitialEntities(ref SystemState state, EntityCommandBuffer ecb)
    {
        foreach (var spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            if (spawner.ValueRO.initialAgents > 1)
            {
                SpawnAgents(spawner, ecb);
            }
            if (spawner.ValueRO.initialGoals > 1)
            {

                SpawnGoals(spawner, ecb);
            }
        }
    }

    private void SpawnAgents(RefRW<SpawnerComponent> spawner, EntityCommandBuffer ecb)
    {
        if (spawner.ValueRO.initialAgents > 1)
        {
            if (spawner.ValueRO.SpawnInterval <= 0)
            {

                Entity agentEntity = ecb.Instantiate(spawner.ValueRO.AgentPrefabEntity);
                spawner.ValueRW.initialAgents -= 1;
                spawner.ValueRW.SpawnInterval = spawner.ValueRO.Timer;
            }
            else
            {
                spawner.ValueRW.SpawnInterval -= .1f;
            }

        }
        else
        {
            spawner.ValueRW.HasSpawnedInitial = true;
        }
    }

    private void SpawnGoals(RefRW<SpawnerComponent> spawner, EntityCommandBuffer ecb)
    {
        ecb.Instantiate(spawner.ValueRO.GoalPrefabEntity);
        spawner.ValueRW.initialGoals -= 1;
    }
}

