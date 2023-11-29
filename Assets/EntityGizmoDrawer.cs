using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EntityGizmoDrawer : MonoBehaviour
{
    private EntityManager _entityManager;
    public float searchRadius = 10.0f; // Example radius, replace with actual value

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Query entities
        var entityQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<LocalTransform>());
        var entities = entityQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);

        foreach (var entity in entities)
        {
            if (_entityManager.HasComponent<LocalTransform>(entity))
            {
                LocalTransform localTransform = _entityManager.GetComponentData<LocalTransform>(entity);
                Vector3 position = new Vector3(localTransform.Position.x, localTransform.Position.y, localTransform.Position.z);

                // Draw a wire sphere for each entity
                Gizmos.DrawWireSphere(position, searchRadius);
            }
        }

        entities.Dispose();
    }
}
