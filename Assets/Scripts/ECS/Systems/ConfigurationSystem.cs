using Unity.Entities;
using UnityEngine;

namespace Borgs
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ConfigurationSystem : SystemBase
    {
        private MainConfig _worldConfig;
        private PhysicsTransformConfig _agentConfig;
        private PhysicsTransformConfig _goalConfig;
        private SpawnConfig _spawnConfig;

        protected override void OnCreate()
        {
            base.OnCreate();
            _agentConfig = Resources.Load<PhysicsTransformConfig>("Configurations/AgentConfig");
            _goalConfig = Resources.Load<PhysicsTransformConfig>("Configurations/GoalConfig");
            _worldConfig = Resources.Load<MainConfig>("Configurations/WorldSettingsConfig");
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

            foreach (var mvData in SystemAPI.Query<RefRW<PhysicsTransformComponent>>())
            {
                UpdateEntitiesBasedOnBoundarySettings(mvData);

                if (mvData.ValueRO.type == ObjectType.Borg)
                {
                    UpdateMoveableObjectData(_agentConfig, mvData);
                }
                else// if (mvData.ValueRO.type == ObjectType.Goal)
                {
                    UpdateMoveableObjectData(_goalConfig, mvData);
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
                spawner.ValueRW.spawnerOn = _spawnConfig.spawnerOn;

                if (spawner.ValueRO.initialAgents > 1)
                {
                    spawner.ValueRW.initialAgents = _spawnConfig.InitialAgentCount;
                }

                if (spawner.ValueRO.initialGoals > 1)
                {
                    spawner.ValueRW.initialGoals = _spawnConfig.InitialGoalCount;
                }
            }
        }

        private static void UpdateMoveableObjectData(PhysicsTransformConfig config, RefRW<PhysicsTransformComponent> mvData)
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
        }
        private void UpdateEntitiesBasedOnBoundarySettings(RefRW<PhysicsTransformComponent> mv_data)
        {
            // Determine which boundary type to use (AgentBoundaryType overrides MoveableObjectBoundaryType)
            switch (mv_data.ValueRO.type)
            {
                case ObjectType.Obstacle:
                    mv_data.ValueRW.boundaryType = _worldConfig.MoveableObjectBoundaryType;
                    break;
                case ObjectType.Borg:
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
}