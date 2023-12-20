using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Borgs
{
    public class ConfigEditorWindow : EditorWindow
    {
        private GameSettingsConfig gameSettingsConfig;
        private PhysicsSettingsConfig borgPhysicsSettingsConfig;
        private PhysicsSettingsConfig goalPhysicsSettingsConfig;
        private bool isDirty;
        [MenuItem("Window/Configurations/Config Editor")]
        public static void ShowWindow()
        {
            GetWindow<ConfigEditorWindow>("Config Editor");
        }

        private void OnEnable()
        {
            // Load configurations when the window is opened or recompiled
            gameSettingsConfig = Resources.Load<GameSettingsConfig>("Configs/GameSettingsConfig");
            borgPhysicsSettingsConfig = Resources.Load<PhysicsSettingsConfig>("Configs/BorgPhysicsSettingsConfig");
            goalPhysicsSettingsConfig = Resources.Load<PhysicsSettingsConfig>("Configs/MoveableObjectPhysicsSettingsConfig");
        }

        private void OnGUI()
        {

            // Display the UI for GameSettingsConfig
            if (gameSettingsConfig != null)
            {
                EditorGUI.BeginChangeCheck(); // Track changes
                DisplayGameSettingsConfig();
                if (EditorGUI.EndChangeCheck() || isDirty)
                {
                    gameSettingsConfig.IsDirty = true;
                    MarkConfigAsDirty(gameSettingsConfig);
                }
            }

            // Display the UI for BorgPhysicsSettingsConfig
            if (borgPhysicsSettingsConfig != null)
            {
                EditorGUI.BeginChangeCheck(); // Track changes
                DisplayPhysicsSettingsConfig(borgPhysicsSettingsConfig, "Borg Physics Settings");
                if (EditorGUI.EndChangeCheck() || isDirty)
                {
                    borgPhysicsSettingsConfig.IsDirty = true;
                    MarkConfigAsDirty(borgPhysicsSettingsConfig);
                }
            }

            // Display the UI for GoalPhysicsSettingsConfig
            if (goalPhysicsSettingsConfig != null)
            {
                EditorGUI.BeginChangeCheck(); // Track changes
                DisplayPhysicsSettingsConfig(goalPhysicsSettingsConfig, "Goal Physics Settings");
                if (EditorGUI.EndChangeCheck() || isDirty)
                {
                    goalPhysicsSettingsConfig.IsDirty = true;
                    MarkConfigAsDirty(goalPhysicsSettingsConfig);
                }
            }

            if (isDirty)
            {
                isDirty = false;
            }
        }

        private void MarkConfigAsDirty(ScriptableObject config)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void DisplayGameSettingsConfig()
        {
            GUILayout.Label("Game Settings", EditorStyles.boldLabel);
            if (gameSettingsConfig != null)
            {
                EditorGUI.BeginChangeCheck();

                // Edit fields for GameSettingsConfig

                GUILayout.Label("Fixed Step Settings", EditorStyles.boldLabel);
                gameSettingsConfig.fixedStep = EditorGUILayout.Toggle("Fixed Step", gameSettingsConfig.fixedStep);
                gameSettingsConfig.fixedStepTime = EditorGUILayout.FloatField("Fixed Step Time", gameSettingsConfig.fixedStepTime);
                GUILayout.Label("", EditorStyles.boldLabel);
                gameSettingsConfig.spawnerOn = EditorGUILayout.Toggle("Spawner On", gameSettingsConfig.spawnerOn);
                gameSettingsConfig.spawnAtOnce = EditorGUILayout.Toggle("Spawn At Once", gameSettingsConfig.spawnAtOnce);
                gameSettingsConfig.InitialBorgCount = EditorGUILayout.IntField("Initial Borg Count", gameSettingsConfig.InitialBorgCount);
                gameSettingsConfig.InitialGoalCount = EditorGUILayout.IntField("Initial Goal Count", gameSettingsConfig.InitialGoalCount);
                gameSettingsConfig.SpawnBatchSize = EditorGUILayout.IntField("Spawn Batch Size", gameSettingsConfig.SpawnBatchSize);
                gameSettingsConfig.SpawnBatchSizeMax = EditorGUILayout.IntField("Spawn Batch Size Max", gameSettingsConfig.SpawnBatchSizeMax);
                gameSettingsConfig.RandomizeSpawnPosition = EditorGUILayout.Toggle("Randomize Spawn Position", gameSettingsConfig.RandomizeSpawnPosition);
                gameSettingsConfig.SpawnSpeed = EditorGUILayout.FloatField("Spawn Speed", gameSettingsConfig.SpawnSpeed);
                gameSettingsConfig.SpawnInterval = EditorGUILayout.FloatField("Spawn Interval", gameSettingsConfig.SpawnInterval);
                gameSettingsConfig.SpawnRadius = EditorGUILayout.FloatField("Spawn Radius", gameSettingsConfig.SpawnRadius);
                gameSettingsConfig.BorgBoundaryType = (BoundaryType)EditorGUILayout.EnumPopup("Borg Boundary Type", gameSettingsConfig.BorgBoundaryType);
                gameSettingsConfig.MoveableObjectBoundaryType = (BoundaryType)EditorGUILayout.EnumPopup("Moveable Object Boundary Type", gameSettingsConfig.MoveableObjectBoundaryType);
                gameSettingsConfig.Reset = EditorGUILayout.Toggle("Reset", gameSettingsConfig.Reset);
                gameSettingsConfig.borgPrefab = EditorGUILayout.ObjectField("Borg Prefab", gameSettingsConfig.borgPrefab, typeof(GameObject), false) as GameObject;
                gameSettingsConfig.goalPrefab = EditorGUILayout.ObjectField("Goal Prefab", gameSettingsConfig.goalPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(gameSettingsConfig);
                    gameSettingsConfig.IsDirty = true;
                }
            }
        }

        private void DisplayPhysicsSettingsConfig(PhysicsSettingsConfig config, string label)
        {
            GUILayout.Label(label, EditorStyles.boldLabel);
            if (config != null)
            {
                UpdateEditorConfig(config);
            }
        }

        private void UpdateEditorConfig(PhysicsSettingsConfig physicsSettingsConfig)
        {
            // Edit fields for PhysicsSettingsConfig
            physicsSettingsConfig.speed = EditorGUILayout.FloatField("Speed", physicsSettingsConfig.speed);
            physicsSettingsConfig.acceleration = EditorGUILayout.FloatField("Acceleration", physicsSettingsConfig.acceleration);
            physicsSettingsConfig.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", physicsSettingsConfig.rotationSpeed);
            physicsSettingsConfig.angularVelocity = EditorGUILayout.FloatField("Angular Velocity", physicsSettingsConfig.angularVelocity);
            physicsSettingsConfig.angularAcceleration = EditorGUILayout.FloatField("Angular Acceleration", physicsSettingsConfig.angularAcceleration);
            physicsSettingsConfig.maxSpeed = EditorGUILayout.FloatField("Max Speed", physicsSettingsConfig.maxSpeed);
            physicsSettingsConfig.maxRotationSpeed = EditorGUILayout.FloatField("Max Rotation Speed", physicsSettingsConfig.maxRotationSpeed);
            physicsSettingsConfig.maxAcceleration = EditorGUILayout.FloatField("Max Acceleration", physicsSettingsConfig.maxAcceleration);
            physicsSettingsConfig.maxAngularAcceleration = EditorGUILayout.FloatField("Max Angular Acceleration", physicsSettingsConfig.maxAngularAcceleration);
            physicsSettingsConfig.maxAngularVelocity = EditorGUILayout.FloatField("Max Angular Velocity", physicsSettingsConfig.maxAngularVelocity);
            physicsSettingsConfig.borgAttractionDistance = EditorGUILayout.FloatField("Borg Attraction Distance", physicsSettingsConfig.borgAttractionDistance);
            physicsSettingsConfig.goalAttractionDistance = EditorGUILayout.FloatField("Goal Attraction Distance", physicsSettingsConfig.goalAttractionDistance);
            physicsSettingsConfig.attractionMultiplier = EditorGUILayout.FloatField("Attraction Multiplier", physicsSettingsConfig.attractionMultiplier);
            physicsSettingsConfig.attractionWeight = EditorGUILayout.FloatField("Attraction Weight", physicsSettingsConfig.attractionWeight);
            physicsSettingsConfig.repulsionDistance = EditorGUILayout.FloatField("Repulsion Distance", physicsSettingsConfig.repulsionDistance);
            physicsSettingsConfig.repulsionWeight = EditorGUILayout.FloatField("Repulsion Weight", physicsSettingsConfig.repulsionWeight);
            physicsSettingsConfig.alignmentDistance = EditorGUILayout.FloatField("Alignment Distance", physicsSettingsConfig.alignmentDistance);
            physicsSettingsConfig.alignmentWeight = EditorGUILayout.FloatField("Alignment Weight", physicsSettingsConfig.alignmentWeight);
            physicsSettingsConfig.BoundaryType = (BoundaryType)EditorGUILayout.EnumPopup("Boundary Type", physicsSettingsConfig.BoundaryType);
            physicsSettingsConfig.usePhysicsTransform = EditorGUILayout.Toggle("Use Physics Transform", physicsSettingsConfig.usePhysicsTransform);


        }
    }
}