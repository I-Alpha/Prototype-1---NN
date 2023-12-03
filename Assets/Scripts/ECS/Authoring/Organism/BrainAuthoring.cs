using Unity.Entities;
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
                var data = new BrainComponent
                {
                    closestEntitiesData = new ClosestEntitiesData()
                };
                AddComponent(entity, data);
            }
        }


    }
}