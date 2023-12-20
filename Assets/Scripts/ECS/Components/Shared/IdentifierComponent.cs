
using Unity.Entities;

namespace Borgs
{
    public struct IdentifierComponent : IComponentData
    {
        public int id;
        public Entity entityRef;
        public ObjectType type;
        public int index;
    }
}