using Unity.Entities;

public struct SpawnerComponent : IComponentData
{
    public Entity AgentPrefabEntity;
    public Entity GoalPrefabEntity;

    public float initialAgents;
    public float initialGoals;

    public float Timer;
    public float SpawnInterval;
    public bool HasSpawnedInitial; // New flag to track initial spawn
}
