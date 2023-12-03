using Unity.Entities;

public struct SpawnerComponent : IComponentData
{
    public Entity AgentPrefabEntity;
    public Entity GoalPrefabEntity;

    public Entity agentsGroupEntity; // Reference to the agents group entity
    public Entity goalsGroupEntity;  // Reference to the goals group entity

    public bool spawnerOn;
    public float initialAgents;
    public float initialGoals;

    public float Timer;
    public float SpawnInterval;
    public bool HasSpawnedInitial; // Flag to track initial spawn

    public bool Reset; // Add this field
    public BoundaryType RandomizeSpawnPosition;

    // You might remove IsDirty if it's not used elsewhere in your code
    public bool IsDirty;
}
