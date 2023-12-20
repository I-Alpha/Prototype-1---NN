using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Borgs
{
    public class BrainAuthoring : MonoBehaviour
    {

        public class BrainAuthoringBaker : Baker<BrainAuthoring>
        {
            public override void Bake(BrainAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new BrainComponent();
                AddComponent(entity, data);
            }
        }


    }
}

namespace Borgs
{
    public struct EntityWithPosition
    {
        public Entity Entity;
        public float Distance;
        public float2 Direction;
        public float3 Position;
        public ObjectType type;
    }

    public struct ClosestEntitiesData
    {
        public EntityWithPosition ClosestBorg;
        public EntityWithPosition ClosestGoal;
        public EntityWithPosition ClosestObstacle;
    }

    public struct BrainComponent : IComponentData
    {
        public ClosestEntitiesData closestEntitiesData;
        public BrainComponent(ClosestEntitiesData closestEntitiesData)
        {
            this.closestEntitiesData = closestEntitiesData;
        }
    }
}