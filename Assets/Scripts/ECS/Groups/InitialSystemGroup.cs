using Borgs;
using Unity.Entities;
using Unity.Scenes;

namespace Borgs
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SceneSystemGroup))]
    public partial class InitialSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddSystemToUpdateList(World.GetOrCreateSystem<ConfigurationSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<InputHandlingSystem>());
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