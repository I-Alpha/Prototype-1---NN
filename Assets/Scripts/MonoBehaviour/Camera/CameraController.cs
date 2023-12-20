using UnityEngine;

namespace Borgs
{

    public class CameraController : MonoBehaviour
    {

        public float zoomTransitionSpeed = 20f;
        public float zoomSensitivity = 1000f;
        private Vector3 targetCameraPosition;  // Target position for smooth panning
        private float targetOrthoSize;         // Target orthographic size for smooth zooming
        public float dragTransitionSpeed = 40f;    // Speed of the smooth transition
        private bool isZoomingIn = false;

        public float panSpeed = 0.5f;  // Increased for higher sensitivity
        public float panSpeedFactor = 0.03f;  // Increased for higher sensitivity
        private Camera cameraComponent;

        private Vector3 dragStartPosition; // To store the starting position of a drag
        private Vector3 dragStartCameraPosition; // To store the camera position at the start of a drag
        private bool isPanning = false; // Flag to indicate if panning is active

        private Vector3 panSmoothDampVelocity;

        void Start()
        {
            cameraComponent = GetComponent<Camera>();
            targetOrthoSize = cameraComponent.orthographicSize;
            targetCameraPosition = transform.position;
            // Ensure initial orthographic size is within new bounds
            targetOrthoSize = Globals.worldBoundaries.ClampCameraSize(targetOrthoSize);
        }

        void Update()
        {
            HandleZoom();
            HandlePanAndMomentum();
            ApplySmoothTransition();
            if (Input.GetKeyDown(KeyCode.C))
            {
                CenterCamera();
            }
        }

        private void HandlePanAndMomentum()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                // Start a new drag
                isPanning = true;
                dragStartPosition = Input.mousePosition;
                dragStartCameraPosition = targetCameraPosition;
            }
            else if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                // End the drag
                isPanning = false;
            }

            if (isPanning)
            {
                // Continue panning while dragging
                Vector3 currentMousePosition = Input.mousePosition;

                // Calculate a smooth adjusted pan speed based on orthographic size
                float adjustedPanSpeed = panSpeed * panSpeedFactor * Mathf.Sqrt(cameraComponent.orthographicSize);

                Vector3 dragOffset = (currentMousePosition - dragStartPosition) * adjustedPanSpeed;

                // Update the target camera position based on the drag offset
                targetCameraPosition = dragStartCameraPosition - dragOffset;

                // Clamp the camera position within boundaries
                targetCameraPosition = Globals.worldBoundaries.ClampCameraPositionWithinBounds(targetCameraPosition, targetOrthoSize, cameraComponent.aspect);
            }

        }

        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                float newTargetSize = targetOrthoSize - (scroll * zoomSensitivity);
                newTargetSize = Globals.worldBoundaries.ClampCameraSize(newTargetSize);

                if (scroll > 0) // Zooming in
                {
                    if (!isZoomingIn)
                    {
                        // Convert the mouse position to a world point
                        Vector3 mousePosition = Input.mousePosition;
                        mousePosition.z = cameraComponent.transform.position.z - cameraComponent.nearClipPlane;
                        //Vector3 mouseWorldPoint = cameraComponent.ScreenToWorldPoint(mousePosition);

                        // Set the target position towards the mouse world point
                        //targetCameraPosition = mouseWorldPoint;
                        isZoomingIn = true;
                    }
                }
                else // Zooming out
                {
                    isZoomingIn = false;
                }

                targetOrthoSize = newTargetSize;
                // Calculate a new position based on the zoom level and keep it within bounds
                targetCameraPosition = Globals.worldBoundaries.ClampCameraPositionWithinBounds(targetCameraPosition, newTargetSize, cameraComponent.aspect);
            }
        }

        void ApplySmoothTransition()
        {
            // Calculate the current center of the camera view
            Vector3 cameraCenter = cameraComponent.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cameraComponent.nearClipPlane));
            cameraCenter.z = transform.position.z; // Adjust the z-coordinate to match the camera's z-position

            // Determine the direction to the target position
            Vector3 directionToTarget = targetCameraPosition - cameraCenter;
            float distanceToTarget = directionToTarget.magnitude;

            // Calculate a proportional speed to move towards the target position
            float proportionalSpeed = (distanceToTarget / targetOrthoSize) * dragTransitionSpeed * cameraComponent.orthographicSize;

            // Apply a camera velocity threshold for both panning and zooming
            float cameraVelocityThreshold = 0.1f * cameraComponent.orthographicSize;

            if (proportionalSpeed < cameraVelocityThreshold)
            {
                proportionalSpeed = 0f; // Stop camera movement if velocity is below the threshold
            }

            // Calculate the new position based on the proportional speed
            Vector3 newPosition = transform.position + (proportionalSpeed * Time.deltaTime * directionToTarget.normalized);

            // Clamp the new position within boundaries and apply it
            transform.position = Globals.worldBoundaries.ClampCameraPositionWithinBounds(newPosition, cameraComponent.orthographicSize, cameraComponent.aspect);

            // Apply smooth zooming
            float PreviousOrthoSize = cameraComponent.orthographicSize;
            cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, targetOrthoSize, Time.deltaTime * zoomTransitionSpeed);
        }


        void CenterCamera()
        {
            Vector3 targetPosition = Globals.worldBoundaries.bounds.center;
            targetPosition.z = transform.position.z;
            Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref panSmoothDampVelocity, 0.1f);
            transform.position = Globals.worldBoundaries.ClampCameraPositionWithinBounds(newPosition, cameraComponent.orthographicSize, cameraComponent.aspect);
        }
    }
}