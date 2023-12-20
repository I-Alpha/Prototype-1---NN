
using Unity.Entities;
using UnityEngine;

namespace Borgs
{
    //empty agent tag component
    public class BorgTagAuthoring : MonoBehaviour
    {
        public class BorgTagBaker : Baker<BorgTagAuthoring>
        {
            public override void Bake(BorgTagAuthoring authoring)
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