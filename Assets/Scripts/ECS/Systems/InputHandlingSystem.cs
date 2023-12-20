using Borgs;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Borgs
{
    [RequireMatchingQueriesForUpdate]
    [CreateAfter(typeof(ConfigurationSystem))]
    [UpdateAfter(typeof(ConfigurationSystem))]
    [UpdateInGroup(typeof(InitialSystemGroup))]
    [BurstCompile]
    public partial class InputHandlingSystem : SystemBase
    {
        private Entity _inputDataEntity;
        private SpawnerDataComponent spawnerData;
        private Entity spawnerDataEntity;

        public static int ExecuteCount = 0;
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(SpawnerDataComponent)));
            _inputDataEntity = EntityManager.CreateEntity(typeof(InputData));
        }

        [BurstCompile]
        protected override void OnUpdate()
        { 
            spawnerDataEntity = SystemAPI.GetSingletonEntity<SpawnerDataComponent>();
            spawnerData = SystemAPI.GetComponent<SpawnerDataComponent>(spawnerDataEntity);

            var inputData = new InputData
            {
                BorgSpawnCommand = new SpawnCommand(),
                GoalSpawnCommand = new SpawnCommand()
            };
            //// Process input
            //if (Input.GetMouseButtonDown(0)) // Left click
            //{
            //    inputData.BorgSpawnCommand = new SpawnCommand
            //    {
            //        ShouldSpawn = true,
            //        Amount = 1,
            //        SpawnPosition = GetMouseWorldPosition()
            //        PrefabEntity = spawnerData.BorgPrefabEntity,
            //    };
            //}
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1)) // Right click
            {
                inputData.GoalSpawnCommand = new SpawnCommand
                {
                    ShouldSpawn = true,
                    Amount = 1,
                    SpawnPosition = GetMouseWorldPosition(),
                };
            }

            if (Input.GetKeyDown(KeyCode.R)) // Press R to reset
            {
                inputData.BorgSpawnCommand = new SpawnCommand
                {
                    ShouldSpawn = true,
                    Amount = spawnerData.initialBorgs,
                    PrefabEntity = spawnerData.BorgPrefabEntity,
                };
                inputData.GoalSpawnCommand = new SpawnCommand
                {
                    ShouldSpawn = true,
                    Amount = spawnerData.initialGoals,
                    PrefabEntity = spawnerData.GoalPrefabEntity,
                };
                spawnerData.Reset = true;
                spawnerData.HasSpawnedInitial = false;
                SystemAPI.SetComponent(spawnerDataEntity, spawnerData);
            }

            // Update the component data 
            SystemAPI.SetComponent(_inputDataEntity, inputData);
        }


        [BurstCompile]
        private float3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            return new float3(worldPosition.x, worldPosition.y, 0); // Assuming 2D, Z is set to 0
        }
    }

    [BurstCompile]
    public struct SpawnCommand
    {
        public int Amount;
        public bool ShouldSpawn;
        public float3 SpawnPosition;
        public BoundaryType RandomizeSpawnPosition;
        public Entity PrefabEntity;
    }

    public struct InputData : IComponentData
    {
        public SpawnCommand BorgSpawnCommand;
        public SpawnCommand GoalSpawnCommand;
    }
}
