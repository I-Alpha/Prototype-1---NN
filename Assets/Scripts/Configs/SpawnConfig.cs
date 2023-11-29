using UnityEngine;

[CreateAssetMenu(fileName = "SpawnConfig", menuName = "Configurations/SpawnConfig")]
public class SpawnConfig : ScriptableObject
{
    public int InitialAgentCount;
    public int InitialGoalCount;
    public float SpawnInterval;
    public float SpawnRadius;
    public bool RandomizeSpawnPosition;
    public float Timer;
    public bool IsDirty;
    private void OnValidate()
    {
        IsDirty = true;
    }
}