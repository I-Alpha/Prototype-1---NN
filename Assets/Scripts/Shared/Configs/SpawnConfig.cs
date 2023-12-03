using UnityEngine;
namespace Borgs
{
    [CreateAssetMenu(fileName = "SpawnConfig", menuName = "Configurations/SpawnConfig")]
    public class SpawnConfig : ScriptableObject
    {
        public int InitialAgentCount;
        public int InitialGoalCount;
        public float SpawnInterval;
        public float SpawnRadius;
        public float Timer;
        public bool IsDirty;
        public bool spawnerOn;
        public bool Reset; // Add this field

        public GameObject agentPrefab;
        public GameObject goalPrefab;

        public BoundaryType RandomizeSpawnPosition;


        public void OnValidate()
        {
            IsDirty = true;
        }
    }
}