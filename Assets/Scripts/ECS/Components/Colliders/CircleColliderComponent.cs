using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
// Other necessary namespaces
namespace Borgs
{
    public class CircleColliderPhysicsAuthoring : MonoBehaviour
    {
        public CircleCollider2D circleCollider2D;

        private void Awake()
        {
            circleCollider2D = GetComponent<CircleCollider2D>();
        }

        public class CircleColliderPhysicsBaker : Baker<CircleColliderPhysicsAuthoring>
        {
            public override void Bake(CircleColliderPhysicsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Convert the 2D points to 3D vertices
                float radius = authoring.circleCollider2D.radius;

                // Add the PhysicsCollider component to the entity
                AddComponent(entity, new CircleCollider2DComponent { radius = radius });
            }
        }
    }
    struct CircleCollider2DComponent : IComponentData
    {
        public float radius;
    }
}