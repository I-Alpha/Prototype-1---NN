using Borgs;
using Unity.Entities;
using UnityEngine;

namespace Borgs
{
    public class GoalTagAuthoring : MonoBehaviour
    {
        public class GoalTagBaker : Baker<GoalTagAuthoring>
        {
            public override void Bake(GoalTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var data = new GoalTag { };
                AddComponent(entity, data);

            }
        }

    }
}

namespace Borgs
{
    //empty agent tag component
    public partial struct GoalTag : IComponentData
    {
    }
}