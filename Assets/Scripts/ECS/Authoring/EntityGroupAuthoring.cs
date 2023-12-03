using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EntityGroupAuthoring : MonoBehaviour
{
    public List<GameObject> entities = new List<GameObject>();
    public string groupTag;
}

public class EntityGroupBaker : Baker<EntityGroupAuthoring>
{
    public override void Bake(EntityGroupAuthoring authoring)
    {
        Entity groupEntity = GetEntity(TransformUsageFlags.None);
        var buffer = AddBuffer<EntityGroupBuffer>(groupEntity);

        foreach (GameObject go in authoring.entities)
        {
            buffer.Add(new EntityGroupBuffer { Entity = GetEntity(go, TransformUsageFlags.Dynamic) });
        }

        AddComponent(groupEntity, new GroupTag { Tag = new FixedString64Bytes(authoring.groupTag) });
    }
}

public struct EntityGroupBuffer : IBufferElementData
{
    public Entity Entity;
}

public struct GroupTag : IComponentData
{
    public FixedString64Bytes Tag;
}
