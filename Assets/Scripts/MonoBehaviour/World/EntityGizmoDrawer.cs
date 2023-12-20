using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Experimental.GlobalIllumination;
using Unity.Collections;

namespace Borgs
{
    public class EntityGizmoDrawer : MonoBehaviour
    {
        private EntityManager _entityManager;
        public float searchRadius = 10.0f; // Example radius, replace with actual value
        private EntityQuery quadtreeComponentEntityQuery;
        private EntityQuery borgQuery;

        void Awake()

        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            quadtreeComponentEntityQuery = _entityManager.CreateEntityQuery(typeof(QuadtreeComponent));
            borgQuery = _entityManager.CreateEntityQuery(typeof(LocalTransform), typeof(BorgTag));
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            Gizmos.color = Color.green;

            // get quadtrees and draw them
            var quadtreeComponentEntity = quadtreeComponentEntityQuery.GetSingletonEntity();
            var quadtreeComponent = _entityManager.GetComponentData<QuadtreeComponent>(quadtreeComponentEntity);
            var borgQuadtree = quadtreeComponent.BorgQuadtree;
            var goalQuadtree = quadtreeComponent.GoalQuadtree;
            var obstacleQuadtree = quadtreeComponent.ObstacleQuadtree;

            // Draw quadtrees
            borgQuadtree.DrawGizmos();
            goalQuadtree.DrawGizmos();
            obstacleQuadtree.DrawGizmos();

            // Draw borgs
            var borgEntities = borgQuery.ToEntityArray(Allocator.TempJob);
            foreach (var borgEntity in borgEntities)
            {
                var borgTransform = _entityManager.GetComponentData<LocalTransform>(borgEntity);
                // Get the borg's transform
                DrawDebugLinesForBorgs(borgTransform, _entityManager.GetComponentData<BrainComponent>(borgEntity));
            }
            borgEntities.Dispose();

        }
        private void DrawDebugLinesForBorgs(LocalTransform transform, BrainComponent brain)
        {
            DrawLineToClosestEntity(transform.Position, brain.closestEntitiesData.ClosestBorg, Color.red);
            DrawLineToClosestEntity(transform.Position, brain.closestEntitiesData.ClosestGoal, Color.yellow);
            DrawLineToClosestEntity(transform.Position, brain.closestEntitiesData.ClosestObstacle, Color.white);
        }
        private void DrawLineToClosestEntity(float3 borgPosition, EntityWithPosition closestEntity, Color lineColor)
        {
            if (closestEntity.Entity != Entity.Null && closestEntity.Distance < float.MaxValue)
            {
                // Convert float3 to Vector3
                Vector3 start = new Vector3(borgPosition.x, borgPosition.y, borgPosition.z);
                Vector3 end = new Vector3(closestEntity.Position.x, closestEntity.Position.y, closestEntity.Position.z);
                // Find the midpoint
                Vector3 midpoint = (start + end) / 2;

                // Draw the line
                Gizmos.color = lineColor;
                Gizmos.DrawLine(start, end);

                // Draw the arrow head at the midpoint
                DrawArrowHead(midpoint, end, lineColor);
            }
        }

        private void DrawArrowHead(Vector3 position, Vector3 direction, Color color)
        {
            // Set the size of the arrow head
            float arrowHeadLength = 0.25f;
            float arrowHeadAngle = 20.0f;

            // Calculate the direction of the arrow
            Vector3 directionNormalized = (direction - position).normalized;

            // Calculate the right and left vectors using cross product to find perpendicular vectors
            Vector3 right = Quaternion.LookRotation(directionNormalized) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(directionNormalized) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

            // Draw the right side of the arrow head
            Gizmos.color = color;
            Gizmos.DrawRay(position, right * arrowHeadLength);

            // Draw the left side of the arrow head
            Gizmos.DrawRay(position, left * arrowHeadLength);
        }
    }
}