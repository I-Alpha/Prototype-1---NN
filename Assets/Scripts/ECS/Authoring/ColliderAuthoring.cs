using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum ColliderType
{
    Box,
    Circle,
    Capsule,
    Mesh,
    Polygon,
}
public partial class ColliderAuthoring : MonoBehaviour
{
    public ColliderType colliderType;


    public void Awake()
    {
        Collider2D collider2D = GetComponent<Collider2D>();

        if (collider2D == null)
        {
            return;
        }
        //check what type of collider it is
        if (collider2D is BoxCollider2D)
        {
            colliderType = ColliderType.Box;
        }
        else if (collider2D is CircleCollider2D)
        {
            colliderType = ColliderType.Circle;
        }
        else if (collider2D is CapsuleCollider2D)
        {
            colliderType = ColliderType.Capsule;
        }
        else if (collider2D is PolygonCollider2D)
        {
            colliderType = ColliderType.Polygon;

        }
    }

    public class ColliderAuthoringtBaker : Baker<ColliderAuthoring>
    {
        public override void Bake(ColliderAuthoring authoring)
        { 

            var entity = GetEntity(TransformUsageFlags.Dynamic);

            switch (authoring.colliderType)
            {
                case ColliderType.Box:
                    BakeBoxCollider(authoring);
                    break;
                case ColliderType.Circle:
                    BakeCircleCollider(authoring);
                    break;
                case ColliderType.Capsule:
                    break;
                case ColliderType.Mesh:
                    break;
                case ColliderType.Polygon:
                    BakePolygonCollider(authoring);
                    break;
                default:
                    break;
            }

        }

        public void BakeBoxCollider(ColliderAuthoring authoring)
        { 
        }

        public void BakeCircleCollider(ColliderAuthoring authoring)
        {
        }
        public void BakePolygonCollider(ColliderAuthoring authoring)
        {
        }
    }


}