using Borgs;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Configurations/GameSettingsConfig")]
public class GameSettingsConfig : ScriptableObject, IConfig
{
    public BoundaryType BorgBoundaryType = BoundaryType.PlayPen;
    public BoundaryType MoveableObjectBoundaryType = BoundaryType.PlayPen;

    public int InitialBorgCount;
    public int InitialGoalCount;

    public float SpawnInterval;

    public float SpawnRadius;
    public float SpawnSpeed;

    public int SpawnBatchSize;
    public int SpawnBatchSizeMax;

    public GameObject borgPrefab;
    public GameObject goalPrefab;

    public bool spawnerOn;
    public bool Reset;
    public bool RandomizeSpawnPosition;

    public bool spawnAtOnce;

    public bool fixedStep;
    public float fixedStepTime;
    public bool IsDirty { get; set; }

    private void OnValidate()
    {
        InitialBorgCount = Mathf.Max(InitialBorgCount, 0);
        InitialGoalCount = Mathf.Max(InitialGoalCount, 0);
        SpawnInterval = Mathf.Max(SpawnInterval, 0);
        SpawnRadius = Mathf.Max(SpawnRadius, 0);
        IsDirty = true;
    }

    public void UpdateConfig()
    {
        // Implement the logic to update the configuration settings
        // Example: You might save these settings to a file or update other game components
    }
}
