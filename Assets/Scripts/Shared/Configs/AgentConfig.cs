using UnityEngine;

namespace Borgs
{
    [CreateAssetMenu(fileName = "AgentConfig", menuName = "Configurations/AgentConfig")]
    public class AgentConfig : ScriptableObject
    {
        public bool IsDirty { get; set; }

        public float MaxSpeed;
        public float MaxRotationSpeed;
        public string behaviorType;
        public bool canReproduce;
        public float size;
        public float energy;
        public float maxEnergy;
        public float MaxAcceleration;

        public float MaxAngularAcceleration;
        public float MaxAngularVelocity;

    }
}