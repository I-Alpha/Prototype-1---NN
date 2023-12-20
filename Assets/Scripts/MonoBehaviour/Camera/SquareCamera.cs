using UnityEngine;

namespace Borgs
{
    [ExecuteInEditMode]
    public class SquareCamera : MonoBehaviour
    {
        private Camera cameraComponent;

        void Awake()
        {
            cameraComponent = GetComponent<Camera>();
        }

        void Update()
        {
            if (cameraComponent != null)
            {
                cameraComponent.aspect = 1.0f; // Square aspect ratio
            }
        }
    }
}