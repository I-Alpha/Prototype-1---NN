using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct IdentifierComponent : IComponentData
{
    public int id;
    public ObjectType type;

    public IdentifierComponent(int id, ObjectType type)
    {
        this.id = id;
        this.type = type;
    }
}
