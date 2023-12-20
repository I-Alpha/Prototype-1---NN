using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using NativeTrees;
using Unity.Transforms;
using UnityEditor.Search;
namespace Borgs
{
    [UpdateInGroup(typeof(CoreSimulationSystemGroup))]
    [CreateBefore(typeof(UpdateBrainSystem))]
    [UpdateBefore(typeof(UpdateBrainSystem))]
    [UpdateAfter(typeof(SpawnerSystem))]
    [BurstCompile]
    public partial struct QuadTreeSystem : ISystem
    {
        NativeQuadtree<Entity> BorgQuadtree;
        NativeQuadtree<Entity> GoalQuadtree;
        NativeQuadtree<Entity> ObstacleQuadtree;


        NativeArray<Entity> entities;
        NativeArray<LocalTransform> transforms;

        EntityQuery boundaryAABBsInfoQuery;
        EntityQuery borgQuery;
        EntityQuery goalQuery;
        EntityQuery obstacleQuery;

        public Entity QuadTreeComponentEnitity;
        BoundaryAABBsInfo boundariesInfo;
        bool initialised;
        public void OnCreate(ref SystemState state)
        {
            initialised = false;
            state.RequireForUpdate<BoundaryAABBsInfo>();
            state.RequireForUpdate<QuadtreeComponent>();
            QuadTreeComponentEnitity = state.EntityManager.CreateEntity(typeof(QuadtreeComponent));
            boundaryAABBsInfoQuery = state.GetEntityQuery(ComponentType.ReadOnly<BoundaryAABBsInfo>());
            borgQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<BorgTag>());
            goalQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<GoalTag>());
            obstacleQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<ObstacleTag>());
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            BorgQuadtree.Dispose();
            GoalQuadtree.Dispose();
            ObstacleQuadtree.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Create entity for each quadtree using ECB

            var borgTreeFilled = false;
            var goalTreeFilled = false;
            var obstacleTreeFilled = false;
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            if (!initialised)
            {
                // Get BoundariesInfo component that holds AABBs of world and playpen
                var boundariesInfoEntity = boundaryAABBsInfoQuery.GetSingletonEntity();
                boundariesInfo = SystemAPI.GetComponent<BoundaryAABBsInfo>(boundariesInfoEntity);

                // Create the quadtrees using the AABB2D
                BorgQuadtree = new NativeQuadtree<Entity>(boundariesInfo.worldAABB, Allocator.Persistent);
                GoalQuadtree = new NativeQuadtree<Entity>(boundariesInfo.worldAABB, Allocator.Persistent);
                ObstacleQuadtree = new NativeQuadtree<Entity>(boundariesInfo.worldAABB, Allocator.Persistent);
                initialised = true;

            }
            else
            {

                borgTreeFilled = RebuildQuadtree(BorgQuadtree, borgQuery, ecb, state);
                goalTreeFilled = RebuildQuadtree(GoalQuadtree, goalQuery, ecb, state);
                obstacleTreeFilled = RebuildQuadtree(ObstacleQuadtree, obstacleQuery, ecb, state);

            }


            // Set component data
            ecb.SetComponent(QuadTreeComponentEnitity, new QuadtreeComponent
            {
                BorgQuadtree = BorgQuadtree,
                BorgQuadtreeFilled = borgTreeFilled,
                GoalQuadtree = GoalQuadtree,
                GoalQuadtreeFilled = goalTreeFilled,
                ObstacleQuadtree = ObstacleQuadtree,
                ObstacleQuadtreeFilled = obstacleTreeFilled,
            });

            // Playback the ECB to apply the changes
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        private bool RebuildQuadtree(NativeQuadtree<Entity> quadtree, EntityQuery query, EntityCommandBuffer entityCommandBuffer, SystemState state)
        {
            quadtree.Clear();
            entities = query.ToEntityArray(Allocator.TempJob);
            transforms = query.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var filled = false;

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var transform = transforms[i];

                // Insert the entity into the quadtree with its new position
                quadtree.InsertPoint(entity, transform.Position.xy);
                //check and add insertedIntoQuadtree component to entity
                if (!state.EntityManager.HasComponent<InsertedIntoQuadTreeTag>(entity))
                {
                    entityCommandBuffer.AddComponent(entity, new InsertedIntoQuadTreeTag());
                }
                filled = true;
            }

            entities.Dispose();
            transforms.Dispose();
            return filled;
        }

        [BurstCompile]
        private AABB2D GetBoundsBasedOnType(BoundaryType type)
        {
            switch (type)
            {
                case BoundaryType.World:
                    return new AABB2D(Globals.worldBoundaries.bottomLeft, Globals.worldBoundaries.topRight);
                case BoundaryType.PlayPen:
                    return new AABB2D(Globals.playPenBoundaries.bottomLeft, Globals.playPenBoundaries.topRight);
                default:
                    return new AABB2D(); // Default or error handling
            }
        }

        public NativeQuadtree<Entity> GetBorg2BorgQuadtree()
        {
            return BorgQuadtree;
        }

        public NativeQuadtree<Entity> GetBorg2GoalQuadtree()
        {
            return GoalQuadtree;
        }

    }

    public struct InsertedIntoQuadTreeTag : IComponentData
    {

    }

    public struct QuadtreeComponent : IComponentData
    {
        public NativeQuadtree<Entity> BorgQuadtree;
        public bool BorgQuadtreeFilled;


        public NativeQuadtree<Entity> GoalQuadtree;
        public bool GoalQuadtreeFilled;

        public NativeQuadtree<Entity> ObstacleQuadtree;
        public bool ObstacleQuadtreeFilled;
    }

}
