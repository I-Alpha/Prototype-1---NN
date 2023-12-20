using UnityEngine;

namespace Borgs
{

    public class RuntimeObjectManager : MonoBehaviour
    {
        public static RuntimeObjectManager Instance;

        private void Awake()
        {
            // Ensure there is only one instance of this manager
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // This ensures the manager persists across scene loads
            DontDestroyOnLoad(gameObject);
        }

        // Function to call when creating a new GameObject
        public GameObject CreateRuntimeGameObject(string name)
        {
            GameObject newObj = new GameObject(name);
            return newObj;
        }

        private void OnDestroy()
        {
            // Destroy all GameObjects created at runtime
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}