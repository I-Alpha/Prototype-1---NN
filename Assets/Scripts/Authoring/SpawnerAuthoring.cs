using Unity.Entities; 
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject goalPregab;
    public SpawnConfig spawnConfig;
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        SpawnerComponent sd = default;

        sd.AgentPrefabEntity = GetEntity(authoring.agentPrefab, TransformUsageFlags.Dynamic);
        sd.GoalPrefabEntity = GetEntity(authoring.goalPregab, TransformUsageFlags.Dynamic);

        sd.initialAgents = authoring.spawnConfig.InitialAgentCount;
        sd.initialGoals = authoring.spawnConfig.InitialGoalCount;
        sd.SpawnInterval = authoring.spawnConfig.SpawnInterval;
        sd.Timer = authoring.spawnConfig.Timer;
        sd.HasSpawnedInitial = false;

        AddComponent(GetEntity(TransformUsageFlags.None), sd);
    }
}
