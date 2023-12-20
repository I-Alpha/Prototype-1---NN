using Borgs;
using NativeTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Borgs
{
    public class BoundaryAABBAuthoring : MonoBehaviour
    {

        public class BoundaryAABBBaker : Baker<BoundaryAABBAuthoring>
        {
            public override void Bake(BoundaryAABBAuthoring authoring)
            {
                // Fetch the AABB2D from the global world boundaries
                AABB2D worldAABB = Globals.worldBoundaries.aabb2D;
                // Fetch the AABB2D from the global world boundaries
                AABB2D playPenAABB = Globals.playPenBoundaries.aabb2D;

                var aabbEntity = GetEntity(TransformUsageFlags.None);

                // Add a component to the entity that includes the AABB2D
                AddComponent(aabbEntity, new BoundaryAABBsInfo
                {
                    worldAABB = worldAABB,
                    playpenAABB = playPenAABB
                });


            }
        }

    }

    public struct BoundaryAABBsInfo : IComponentData
    {
        public AABB2D worldAABB;
        public AABB2D playpenAABB;

    }
}