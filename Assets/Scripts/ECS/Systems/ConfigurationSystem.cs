using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Borgs
{
    [RequireMatchingQueriesForUpdate]
    [CreateBefore(typeof(InputHandlingSystem))]
    [UpdateBefore(typeof(InputHandlingSystem))]
    [UpdateInGroup(typeof(InitialSystemGroup))]
    public partial class ConfigurationSystem : SystemBase
    {
        private GameSettingsConfig _gameSettingsConfig;
        private PhysicsSettingsConfig _goalPhysicsSettingsConfig;
        private PhysicsSettingsConfig _borgPhysicsSettingsConfig;


        private Entity _fixedTimeSettingsEntity;

        private Entity _defaultBorgPhysicsTransformComponenentEntity;
        private Entity _defaultGoalPhysicsTransformComponenentEntity;




        protected override void OnCreate()
        {
            base.OnCreate();
            _gameSettingsConfig = Resources.Load<GameSettingsConfig>("Configs/GameSettingsConfig");
            _borgPhysicsSettingsConfig = Resources.Load<PhysicsSettingsConfig>("Configs/BorgPhysicsSettingsConfig");
            _goalPhysicsSettingsConfig = Resources.Load<PhysicsSettingsConfig>("Configs/MoveableObjectPhysicsSettingsConfig");

            //create fixedTimesettings entity
            _fixedTimeSettingsEntity = EntityManager.CreateEntity(typeof(FixedTimeSettings));

            // Create default PhysicsTransform component data entity for borgs
            _defaultBorgPhysicsTransformComponenentEntity = EntityManager.CreateEntity(typeof(DefaultBorgPhysicsTransformComponent));
            EntityManager.SetComponentData(_defaultBorgPhysicsTransformComponenentEntity, new DefaultBorgPhysicsTransformComponent
            {
                Settings = GetPhysicsTransformComponentFromConfig(_borgPhysicsSettingsConfig)
            });

            // Create default PhysicsTransform component data entity for goals
            _defaultGoalPhysicsTransformComponenentEntity = EntityManager.CreateEntity(typeof(DefaultGoalPhysicsTransformComponent));
            EntityManager.SetComponentData(_defaultGoalPhysicsTransformComponenentEntity, new DefaultGoalPhysicsTransformComponent
            {
                Settings = GetPhysicsTransformComponentFromConfig(_goalPhysicsSettingsConfig)
            });
        }

        private PhysicsTransformComponent GetPhysicsTransformComponentFromConfig(PhysicsSettingsConfig settings)
        {
            return new PhysicsTransformComponent
            {
                acceleration = settings.acceleration,
                rotationSpeed = settings.rotationSpeed,
                angularAcceleration = settings.angularAcceleration,
                angularVelocity = settings.angularVelocity,
                speed = settings.speed,
                maxSpeed = settings.maxSpeed,
                maxRotationSpeed = settings.maxRotationSpeed,
                maxAcceleration = settings.maxAcceleration,
                maxAngularAcceleration = settings.maxAngularAcceleration,
                maxAngularVelocity = settings.maxAngularVelocity,
                boundaryType = settings.BoundaryType,
                borgAttractionDistance = settings.borgAttractionDistance,
                alignmentDistance = settings.alignmentDistance,
                repulsionDistance = settings.repulsionDistance,
                repulsionWeight = settings.repulsionWeight,
                attractionWeight = settings.attractionWeight,
                alignmentWeight = settings.alignmentWeight,
                attractionMultiplier = settings.attractionMultiplier,
                goalAttractionDistance = settings.goalAttractionDistance,

            };
        }

        protected override void OnUpdate()
        {

            if (_gameSettingsConfig.IsDirty)
            {
                // Update the spawn system settings
                foreach (var spawner in SystemAPI.Query<RefRW<SpawnerDataComponent>>())
                {
                    spawner.ValueRW.SpawnInterval = _gameSettingsConfig.SpawnInterval;
                    spawner.ValueRW.spawnerOn = _gameSettingsConfig.spawnerOn;
                    spawner.ValueRW.initialBorgs = _gameSettingsConfig.InitialBorgCount;
                    spawner.ValueRW.initialGoals = _gameSettingsConfig.InitialGoalCount;
                    spawner.ValueRW.SpawnBatchSize = _gameSettingsConfig.SpawnBatchSize;
                    spawner.ValueRW.SpawnBatchSizeMax = _gameSettingsConfig.SpawnBatchSizeMax;
                    spawner.ValueRW.SpawnRadius = _gameSettingsConfig.SpawnRadius;
                    spawner.ValueRW.RandomizeSpawnPosition = _gameSettingsConfig.RandomizeSpawnPosition;
                    spawner.ValueRW.Reset = _gameSettingsConfig.Reset;
                    spawner.ValueRW.BorgBoundaryType = _gameSettingsConfig.BorgBoundaryType;
                    spawner.ValueRW.MoveableObjectBoundaryType = _gameSettingsConfig.MoveableObjectBoundaryType;
                    spawner.ValueRW.spawnAtOnce = _gameSettingsConfig.spawnAtOnce;
                    spawner.ValueRW.IsDirty = true;
                    spawner.ValueRW.useBorgPhysicsTransform = _borgPhysicsSettingsConfig.usePhysicsTransform;
                    spawner.ValueRW.useGoalPhysicsTransform = _goalPhysicsSettingsConfig.usePhysicsTransform;
                }

                // Update the fixed time settings
                SystemAPI.SetComponent(_fixedTimeSettingsEntity, new FixedTimeSettings
                {
                    fixedStepTime = _gameSettingsConfig.fixedStepTime,
                    fixedStep = _gameSettingsConfig.fixedStep
                });

                _borgPhysicsSettingsConfig.IsDirty = true;
                _goalPhysicsSettingsConfig.IsDirty = true;
                _gameSettingsConfig.IsDirty = false;

            }
            if (_borgPhysicsSettingsConfig.IsDirty)
            {

                // Update the spawn system settings
                foreach (var spawner in SystemAPI.Query<RefRW<SpawnerDataComponent>>())
                {
                    spawner.ValueRW.IsDirty = true;
                }

                // Update the borg physics settings
                foreach (var (mvData, _) in SystemAPI.Query<RefRW<PhysicsTransformComponent>, RefRO<BorgTag>>())
                {

                    UpdateMoveableObjectData(_borgPhysicsSettingsConfig, mvData);
                }

                //update default physics transform component data for borgs
                EntityManager.SetComponentData(_defaultBorgPhysicsTransformComponenentEntity, new DefaultBorgPhysicsTransformComponent
                {
                    Settings = GetPhysicsTransformComponentFromConfig(_borgPhysicsSettingsConfig)
                });

                _borgPhysicsSettingsConfig.IsDirty = false;
            }
            if (_goalPhysicsSettingsConfig.IsDirty)
            {
                // Update the goal physics settings
                foreach (var (mvData, _) in SystemAPI.Query<RefRW<PhysicsTransformComponent>, RefRO<GoalTag>>())
                {
                    UpdateMoveableObjectData(_goalPhysicsSettingsConfig, mvData);
                }
                //update default physics transform component data for goals
                EntityManager.SetComponentData(_defaultGoalPhysicsTransformComponenentEntity, new DefaultGoalPhysicsTransformComponent
                {
                    Settings = GetPhysicsTransformComponentFromConfig(_goalPhysicsSettingsConfig)
                });

                _goalPhysicsSettingsConfig.IsDirty = false;
            }

        }

        private static void UpdateMoveableObjectData(PhysicsSettingsConfig config, RefRW<PhysicsTransformComponent> mvData)
        {
            // Update MoveableObject data based on PhysicsSettingsConfig
            mvData.ValueRW.acceleration = config.acceleration;
            mvData.ValueRW.rotationSpeed = config.rotationSpeed;
            mvData.ValueRW.angularAcceleration = config.angularAcceleration;
            mvData.ValueRW.angularVelocity = config.angularVelocity;
            mvData.ValueRW.speed = config.speed;
            mvData.ValueRW.maxSpeed = config.maxSpeed;
            mvData.ValueRW.maxRotationSpeed = config.maxRotationSpeed;
            mvData.ValueRW.maxAcceleration = config.maxAcceleration;
            mvData.ValueRW.maxAngularAcceleration = config.maxAngularAcceleration;
            mvData.ValueRW.maxAngularVelocity = config.maxAngularVelocity;
            mvData.ValueRW.boundaryType = config.BoundaryType;
            mvData.ValueRW.borgAttractionDistance = config.borgAttractionDistance;
            mvData.ValueRW.goalAttractionDistance = config.goalAttractionDistance;
            mvData.ValueRW.attractionMultiplier = config.attractionMultiplier;
            mvData.ValueRW.alignmentWeight = config.alignmentWeight;
            mvData.ValueRW.alignmentDistance = config.alignmentDistance;
            mvData.ValueRW.alignmentDistance = config.alignmentDistance;
            mvData.ValueRW.repulsionDistance = config.repulsionDistance;
            mvData.ValueRW.repulsionWeight = config.repulsionWeight;
            mvData.ValueRW.attractionWeight = config.attractionWeight;
            mvData.ValueRW.isDirty = true;

        }
    }

    //create fixedTimeSettings struct
    public struct FixedTimeSettings : IComponentData
    {
        public float fixedStepTime;
        public bool fixedStep;
    }
}
