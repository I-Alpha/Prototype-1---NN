using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Experimental.GlobalIllumination;

namespace Borgs
{
    public class EntityGizmoDrawer : MonoBehaviour
    {
        private EntityManager _entityManager;
        public float searchRadius = 10.0f; // Example radius, replace with actual value

        void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            Gizmos.color = Color.green;
        }
        private void DrawDebugLinesForAgents(LocalTransform transform, BrainComponent brain)
        {
            DrawLineToClosestEntity(transform.Position, brain.closestEntitiesData.ClosestAgent, Color.red);
            DrawLineToClosestEntity(transform.Position, brain.closestEntitiesData.ClosestGoal, Color.yellow);
            DrawLineToClosestEntity(transform.Position, brain.closestEntitiesData.ClosestObstacle, Color.white);
        }
        private void DrawLineToClosestEntity(float3 agentPosition, EntityWithPosition closestEntity, Color lineColor)
        {
            if (closestEntity.Entity != Entity.Null && closestEntity.Distance < float.MaxValue)
            {
                // Convert float3 to Vector3
                Vector3 start = new Vector3(agentPosition.x, agentPosition.y, agentPosition.z);
                Vector3 end = new Vector3(closestEntity.Position.x, closestEntity.Position.y, closestEntity.Position.z);

                Gizmos.color = lineColor;
                Gizmos.DrawLine(start, end);
            }
        }
    }
}