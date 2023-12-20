using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Borgs
{
    public struct SpawnerDataComponent : IComponentData
    {
        [SerializeField]
        public Entity BorgPrefabEntity;

        [SerializeField]
        public Entity GoalPrefabEntity;

        public bool spawnerOn;

        public int initialBorgs;
        public int initialGoals;
        public int SpawnedBorgsCount; // Counter for spawned borgs
        public int SpawnedGoalsCount; // Counter for spawned goals
        public int BorgSpawnBatchSize; // Counter for spawned borgs
        public int GoalSpawnBatchSize; // Counter for spawned goals
        public int SpawnBatchSize;

        public int SpawnBatchSizeMax;

        public int SpawnBatchSizeMin;
        public float SpawnRadius;
        public bool spawnAtOnce;

        public float Timer;
        public float SpawnInterval;
        public bool HasSpawnedInitial; // Flag to track initial spawn

        public BoundaryType BorgBoundaryType;
        public BoundaryType MoveableObjectBoundaryType;

        public bool Reset; // Add this field

        public float3 SpawnPosition;
        public bool RandomizeSpawnPosition;
        public UnityEngine.Hash128 SubSceneID;
        public bool useGoalPhysicsTransform;
        public bool useBorgPhysicsTransform;

        public bool IsDirty;
    }
}