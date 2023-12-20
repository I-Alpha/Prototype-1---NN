using Unity.Entities;

namespace Borgs
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class CoreSimulationSystemGroup : ComponentSystemGroup
    {

        protected override void OnCreate()
        {
            base.OnCreate();
            AddSystemToUpdateList(World.GetOrCreateSystem<SpawnerSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<QuadTreeSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateBrainSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<PhysicsTransformSystem>());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}