using NativeTrees;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Borgs
{
    public struct EntityWithPosition
    {
        public Entity Entity;
        public float Distance;
        public float Direction;
        public float3 Position;
        public ObjectType type;
    }

    public struct ClosestEntitiesData
    {
        public EntityWithPosition ClosestAgent;
        public EntityWithPosition ClosestGoal;
        public EntityWithPosition ClosestObstacle;
    }

    [BurstCompile]
    public struct FindClosestEntityJob : IJobChunk
    {
        public float SearchRadius;

        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<int> ChunkBaseEntityIndices;

        public EntityCommandBuffer.ParallelWriter Ecb;

        [ReadOnly] public EntityTypeHandle EntityTypeHandle;
        [ReadOnly] public NativeHashMap<int, Entity> EntityIndexToEntityMap;
        [ReadOnly] public NativeHashMap<int, LocalTransform> EntityIndexToLocalTransformMap;
        [ReadOnly] public NativeHashMap<int, PhysicsTransformComponent> EntityIndexToMoveableObjectMap;

        [ReadOnly] public ComponentTypeHandle<BrainComponent> BrainComponentTypeHandle;
        [ReadOnly] public ComponentTypeHandle<LocalTransform> LocalTransformTypeHandle;
        [ReadOnly] public ComponentTypeHandle<PhysicsTransformComponent> MoveableObjectDataTypeHandle;

        [ReadOnly] public NativeQuadtree<int> Quadtree;

        int agentCount;
        int goalCount;
        int obstacleCount;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var chunkLocalTransforms = chunk.GetNativeArray(ref LocalTransformTypeHandle);
            var chunkMoveableObjectData = chunk.GetNativeArray(ref MoveableObjectDataTypeHandle);
            var chunkEntities = chunk.GetNativeArray(EntityTypeHandle);


            NearestVisitor visitor = new NearestVisitor
            {
                NearbyEntities = new NativeList<int>(Allocator.Temp)
            };

            DistanceProvider distanceProvider = new DistanceProvider();

            for (int i = 0; i < chunk.Count; i++)
            {
                //    if (chunkMoveableObjectData[i].type != ObjectType.Borg)
                //        continue;

                //    var writeData = new ClosestEntitiesData
                //    {
                //        ClosestAgent = InitializeEntityWithPosition(),
                //        ClosestGoal = InitializeEntityWithPosition(),
                //        ClosestObstacle = InitializeEntityWithPosition()
                //    };


                //    float3 currentEntityPosition = chunkLocalTransforms[i].Position;

                //    //if (Quadtree.TryGetNearestAABB(entityPosition.xy, 100, out int nearestIndex)){ 
                //    //    //make sure the nearest entity is not the current entity, if so, skip it
                //    //    if (nearestIndex == chunkEntities[i].Index) continue;
                //    //    if (EntityIndexToLocalTransformMap.TryGetValue(nearestIndex, out LocalTransform nearestTransform))
                //    //    {
                //    //        var distance = math.distance(entityPosition, nearestTransform.Position);
                //    //        var targetType = EntityIndexToMoveableObjectMap[nearestIndex].type;
                //    //        UpdateClosestEntity(ref writeData, targetType, chunkEntities[i], distance, chunkLocalTransforms[i].Position);
                //    //    }
                //    //} 

                //    Entity currentEntity = chunkEntities[i];

                //    // Get the nearby entities from the quadtree
                //    Quadtree.Nearest(currentEntityPosition.xy, 100, ref visitor, default(NativeQuadtreeExtensions.AABBDistanceSquaredProvider<int>));

                //    agentCount = 0;
                //    goalCount = 0;
                //    obstacleCount = 0;

                //    foreach (var entityIndex in visitor.NearbyEntities)
                //    {
                //        UnityEngine.Debug.Log(entityIndex + " vs " + currentEntity.Index);
                //        if (entityIndex == currentEntity.Index)
                //            continue;

                //        //get entity position  from chunk
                //        float distance = math.distance(currentEntityPosition, EntityIndexToLocalTransformMap[entityIndex].Position);

                //        ObjectType targetType = EntityIndexToMoveableObjectMap[entityIndex].type;

                //        UnityEngine.Debug.Log("Borg - " + agentCount);
                //        switch (targetType)
                //        {
                //            case ObjectType.Borg:
                //                if (agentCount >= 1)
                //                    continue;

                //                break;
                //            case ObjectType.Goal:
                //                if (goalCount >= 1)
                //                    continue;
                //                break;
                //            case ObjectType.Obstacle:
                //                if (obstacleCount >= 1)
                //                    continue;
                //                break;
                //            default:
                //                break;
                //        }

                //        UpdateClosestEntity(ref writeData,
                //            targetType, EntityIndexToEntityMap[entityIndex],
                //            distance,
                //            EntityIndexToLocalTransformMap[entityIndex].Position
                //        );
                //    }
                //    visitor.NearbyEntities.Clear();



                //    // Add or set the BrainComponent with updated closest entities data
                //    var brainComponent = new BrainComponent
                //    {
                //        closestEntitiesData = writeData
                //    };

                //    Ecb.SetComponent(unfilteredChunkIndex, chunkEntities[i], brainComponent);
            }

            visitor.NearbyEntities.Dispose();



        }

        public struct NearestVisitor : IQuadtreeNearestVisitor<int>
        {
            public NativeList<int> NearbyEntities; // Changed from NativeList<int> to NativeList<Entity>
            public int entityCount;
            public bool OnVist(int id)
            {
                NearbyEntities.Add(id); // Directly adding the Entity
                entityCount++;
                return entityCount < 10;
            }
        }

        public struct DistanceProvider : IQuadtreeDistanceProvider<int>
        {
            public float DistanceSquared(float2 point, int entityIndex, AABB2D bounds)
            {
                //caluclate distance between point and bounds
                var distance = math.distancesq(point, bounds.Center);
                return distance;
            }
        }
        private EntityWithPosition InitializeEntityWithPosition()
        {
            return new EntityWithPosition
            {
                Entity = Entity.Null,
                Distance = float.MaxValue,
                Position = float3.zero
            };
        }

        private void UpdateClosestEntity(ref ClosestEntitiesData data, ObjectType targetType, Entity targetEntity, float distance, float3 position)
        {
            var entityWithPosition = new EntityWithPosition { Entity = targetEntity, Distance = distance, Position = position };

            switch (targetType)
            {
                case ObjectType.Borg:
                    //if (distance <= data.ClosestAgent.Distance)
                    //{
                    data.ClosestAgent = entityWithPosition;
                    agentCount++;

                    UnityEngine.Debug.Log("updated! - " + distance);
                    UnityEngine.Debug.Log("agentCount - " + agentCount.ToString());
                    UnityEngine.Debug.Log("position - " + position.ToString());
                    //}
                    break;
                case ObjectType.Goal:
                    // if (distance <= data.ClosestGoal.Distance)
                    //  {
                    data.ClosestGoal = entityWithPosition;
                    goalCount++;
                    //    }
                    break;
                case ObjectType.Obstacle:
                    //       if (distance <= data.ClosestObstacle.Distance)
                    //        {
                    data.ClosestObstacle = entityWithPosition;
                    obstacleCount++;
                    //     }
                    break;
            }
        }
    }
}