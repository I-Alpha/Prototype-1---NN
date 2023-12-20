using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace Borgs
{
    public class BorgMovementCommandAuthoring : MonoBehaviour
    {
        public float Thrust;
        public float Rotation;

        public class BorgMovementCommandAuthoringBaker : Baker<BorgMovementCommandAuthoring>
        {
            public override void Bake(BorgMovementCommandAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var data = new BorgMovementCommandComponent
                {
                    Thrust = authoring.Thrust,
                    Rotation = authoring.Rotation
                };
                AddComponent(entity, data);
            }
        }

    }
    public struct BorgMovementCommandComponent : IComponentData
    {
        public float Thrust;
        public float Rotation;
    }
}