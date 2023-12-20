
using Unity.Entities;
using UnityEngine;

namespace Borgs
{
    //empty agent tag component
    public class BoundaryTagAuthoring : MonoBehaviour
    {
        public class BoundaryTagAuthoringBaker : Baker<BoundaryTagAuthoring>
        {
            public override void Bake(BoundaryTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new BorgTag { };
                AddComponent(entity, data);

            }
        }
    }
}

namespace Borgs
{
    public partial struct BorgTag : IComponentData
    {
    }
}