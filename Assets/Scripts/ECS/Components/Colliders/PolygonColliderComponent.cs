using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
// Other necessary namespaces

namespace Borgs
{
    public class PolygonColliderPhysicsAuthoring : MonoBehaviour
    {
        public PolygonCollider2D polygonCollider2D;

        private void Awake()
        {
            polygonCollider2D = GetComponent<PolygonCollider2D>();
        }
        public class PolygonColliderPhysicsBaker : Baker<PolygonColliderPhysicsAuthoring>
        {
            public override void Bake(PolygonColliderPhysicsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<PolygonColliderBlobAsset>();
                var vertices = builder.Allocate(ref root.vertices, authoring.polygonCollider2D.points.Length);

                for (int i = 0; i < authoring.polygonCollider2D.points.Length; i++)
                {
                    vertices[i] = new float3(authoring.polygonCollider2D.points[i].x, authoring.polygonCollider2D.points[i].y, 0f);
                }

                BlobAssetReference<PolygonColliderBlobAsset> blobAssetReference = builder.CreateBlobAssetReference<PolygonColliderBlobAsset>(Allocator.Persistent);
                builder.Dispose();

                AddComponent(entity, new Polygon2DColliderPhysicsComponent
                {
                    colliderBlobAsset = blobAssetReference
                });
            }
        }

        public struct Polygon2DColliderPhysicsComponent : IComponentData
        {
            public BlobAssetReference<PolygonColliderBlobAsset> colliderBlobAsset;
        }
        public struct PolygonColliderBlobAsset
        {
            public BlobArray<float3> vertices;
        }
    }
}