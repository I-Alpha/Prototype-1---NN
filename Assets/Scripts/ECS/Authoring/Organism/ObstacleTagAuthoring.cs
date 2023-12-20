using Borgs;
using Unity.Entities;
using UnityEngine;

namespace Borgs
{
    //empty agent tag component
    public class ObstacleTagAuthoring : MonoBehaviour
    {
        public class ObstacleTagBaker : Baker<ObstacleTagAuthoring>
        {
            public override void Bake(ObstacleTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var data = new ObstacleTag { };
                AddComponent(entity, data);

            }
        }
    }
}

namespace Borgs
{
    public partial struct ObstacleTag : IComponentData
    {
    }
}