using Unity.Entities;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public partial class InitilisationSystemGroup : ComponentSystemGroup
{

    protected override void OnCreate()
    {
        base.OnCreate();
        //AddSystemToUpdateList(World.GetOrCreateSystem<NativeQuadtreeSystem>());
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
