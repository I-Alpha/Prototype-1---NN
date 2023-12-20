
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using UnityEngine;

namespace Borgs
{
    public class SpawnDataComponentAuthoring : MonoBehaviour
    {
        public GameSettingsConfig gameSettingsConfig;

        public int goalSpawnBatchSize = 1;
        public int borgSpawnBatchSize = 1;

        public Transform SpawnPositionTransform;
    }

    public class SpawnerComponentBaker : Baker<SpawnDataComponentAuthoring>
    {
        public override void Bake(SpawnDataComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnerDataComponent
            {
                initialBorgs = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.InitialBorgCount : 0,
                initialGoals = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.InitialGoalCount : 0,
                SpawnInterval = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.SpawnInterval : 0,
                Timer = 0,
                spawnerOn = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.spawnerOn : true,
                IsDirty = false,
                HasSpawnedInitial = false,
                Reset = false,
                RandomizeSpawnPosition = authoring.gameSettingsConfig != null && authoring.gameSettingsConfig.RandomizeSpawnPosition,
                SpawnPosition = authoring.SpawnPositionTransform != null ? new float3(authoring.SpawnPositionTransform.position.x, authoring.SpawnPositionTransform.position.y, 0) : float3.zero,
                BorgBoundaryType = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.BorgBoundaryType : BoundaryType.None,
                MoveableObjectBoundaryType = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.MoveableObjectBoundaryType : BoundaryType.None,
                //get prefabs from configs
                BorgPrefabEntity = authoring.gameSettingsConfig.borgPrefab != null ? GetEntity(authoring.gameSettingsConfig.borgPrefab, TransformUsageFlags.Dynamic) : Entity.Null,
                GoalPrefabEntity = authoring.gameSettingsConfig.goalPrefab != null ? GetEntity(authoring.gameSettingsConfig.goalPrefab, TransformUsageFlags.Dynamic) : Entity.Null,
                SpawnedBorgsCount = 0,
                SpawnedGoalsCount = 0,
                SpawnBatchSize = authoring.gameSettingsConfig.SpawnBatchSize,
                BorgSpawnBatchSize = authoring.borgSpawnBatchSize,
                GoalSpawnBatchSize = authoring.goalSpawnBatchSize,
                SpawnBatchSizeMax = 10000,
                SpawnBatchSizeMin = 1,
                SpawnRadius = authoring.gameSettingsConfig != null ? authoring.gameSettingsConfig.SpawnRadius : 1,
                spawnAtOnce = authoring.gameSettingsConfig.spawnAtOnce,
            });

        }
    }
}