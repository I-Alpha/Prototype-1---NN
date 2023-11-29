using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public partial class InputHandlingSystem : SystemBase
{
    private Entity _inputDataEntity;

    protected override void OnCreate()
    {
        base.OnCreate();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _inputDataEntity = entityManager.CreateEntity(typeof(InputData));
    }

    protected override void OnUpdate()
    {
        var inputData = new InputData
        {
            AgentSpawnCommand = new SpawnCommand(),
            GoalSpawnCommand = new SpawnCommand()
        };

        //// Process input
        //if (Input.GetMouseButtonDown(0)) // Left click
        //{
        //    inputData.AgentSpawnCommand = new SpawnCommand
        //    {
        //        ShouldSpawn = true,
        //        Amount = 1,
        //        SpawnPosition = GetMouseWorldPosition()
        //    };
        //}
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1)) // Right click
        {
            inputData.GoalSpawnCommand = new SpawnCommand
            {
                ShouldSpawn = true,
                Amount = 1,
                SpawnPosition = GetMouseWorldPosition()
            };
        }

        // Update the component data
        EntityManager.SetComponentData(_inputDataEntity, inputData);
    }

    private float3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return new float3(worldPosition.x, worldPosition.y, 0); // Assuming 2D, Z is set to 0
    }
}
public struct SpawnCommand
{
    public bool ShouldSpawn;
    public int Amount;
    public float3 SpawnPosition;
}

public struct InputData : IComponentData
{
    public SpawnCommand AgentSpawnCommand;
    public SpawnCommand GoalSpawnCommand;
}