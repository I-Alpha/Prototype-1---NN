using Unity.Entities;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public partial class ConfigurationSystem : SystemBase
{
    private WorldSettingsConfig _worldConfig;
    private MoveableObjectConfig _agentConfig;
    private MoveableObjectConfig _goalConfig;
    private SpawnConfig _spawnConfig;



    protected override void OnCreate()
    {
        base.OnCreate();
        _agentConfig = Resources.Load<MoveableObjectConfig>("Configurations/AgentConfig");
        _goalConfig = Resources.Load<MoveableObjectConfig>("Configurations/GoalConfig");
        _worldConfig = Resources.Load<WorldSettingsConfig>("Configurations/WorldSettingsConfig");
        _spawnConfig = Resources.Load<SpawnConfig>("Configurations/SpawnConfig");
    }
    protected override void OnUpdate()
    {

        if (_worldConfig.IsDirty || _agentConfig.IsDirty || _goalConfig.IsDirty)
        {
            UpdateEntitiesMoveableObjectData();
            // Reset the IsDirty flags
            _agentConfig.IsDirty = false;
            _goalConfig.IsDirty = false;
            _worldConfig.IsDirty = false;
        }

        if (_spawnConfig.IsDirty)
        {
            UpdateSpawnerSettings();
            _spawnConfig.IsDirty = false;
        }
    }

    private void UpdateEntitiesMoveableObjectData()
    {

        foreach (var mvData in SystemAPI.Query<RefRW<MoveableObjectComponent>>())
        {
            UpdateEntitiesBasedOnBoundarySettings(mvData);

            if (mvData.ValueRO.type == ObjectType.Agent)
            {
                updateMoveableObjectData(_agentConfig, mvData);
            }
            else// if (mvData.ValueRO.type == ObjectType.Goal)
            {
                updateMoveableObjectData(_goalConfig, mvData);
            }
            //else
            //{
            //    Debug.LogError("Unknown object type");
            //}
        }
    }


    private void UpdateSpawnerSettings()
    {
        // Update the spawn system
        foreach (var spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            spawner.ValueRW.SpawnInterval = _spawnConfig.SpawnInterval;
            spawner.ValueRW.Timer = _spawnConfig.Timer;
        }
    }

    private static void updateMoveableObjectData(MoveableObjectConfig config, RefRW<MoveableObjectComponent> mvData)
    {
        mvData.ValueRW.speed = config.speed;
        mvData.ValueRW.rotationSpeed = config.rotationSpeed;
        mvData.ValueRW.acceleration = config.acceleration;
        mvData.ValueRW.angularAcceleration = config.angularAcceleration;
        mvData.ValueRW.angularVelocity = config.angularVelocity;

        mvData.ValueRW.maxSpeed = config.maxSpeed;
        mvData.ValueRW.maxRotationSpeed = config.maxRotationSpeed;
        mvData.ValueRW.maxAcceleration = config.maxAcceleration;
        mvData.ValueRW.maxAngularAcceleration = config.maxAngularAcceleration;
        mvData.ValueRW.maxAngularVelocity = config.maxAngularVelocity;

        mvData.ValueRW.boundaryType = config.boundaryType;
    }
    private void UpdateEntitiesBasedOnBoundarySettings(RefRW<MoveableObjectComponent> mv_data)
    {
        // Determine which boundary type to use (AgentBoundaryType overrides MoveableObjectBoundaryType)
        switch (mv_data.ValueRO.type)
        {
            case ObjectType.Obstacle:
                mv_data.ValueRW.boundaryType = _worldConfig.MoveableObjectBoundaryType;
                break;
            case ObjectType.Agent:
                mv_data.ValueRW.boundaryType = _worldConfig.AgentBoundaryType;
                break;
            case ObjectType.Goal:
                mv_data.ValueRW.boundaryType = _worldConfig.MoveableObjectBoundaryType;
                break;
            default:
                break;
        }

    }
}
