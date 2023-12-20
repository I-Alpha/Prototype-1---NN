using Unity.Entities;
using UnityEngine;

namespace Borgs
{
    public class IdentifierAuthoring : MonoBehaviour
    {
        public ObjectType Type;
    }

    public class IdentifierBaker : Baker<IdentifierAuthoring>
    {
        public override void Bake(IdentifierAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new IdentifierComponent
            {
                entityRef = entity,
                id = entity.Index,
                type = authoring.Type,
                index = entity.Index
            };
            AddComponent(entity, data);
        }
    }
}


public enum ObjectType
{
    Obstacle,
    Borg,
    Goal,
    // Add other types as needed
}