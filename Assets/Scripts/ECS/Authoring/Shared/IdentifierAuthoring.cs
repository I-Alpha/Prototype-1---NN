using Unity.Entities;
using UnityEngine;

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
            id = entity.Index,  
            type = authoring.Type,
            index = entity.Index
        };
        AddComponent(entity, data);
    }
}