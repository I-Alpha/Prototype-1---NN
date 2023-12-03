using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace Borgs
{
    public static class Globals
    {
        public static WorldBoundaries2D worldBoundaries = new WorldBoundaries2D(); // Initialized with default values
        public static PlaneBoundaries2D planeBoundaries = new PlaneBoundaries2D(); // Initialized with default values
        public static PlayPenBoundaries2D playPenBoundaries = new PlayPenBoundaries2D(); // Initialized with default values

        // Method to configure world boundaries. You can call this to update boundaries.
        public static void ConfigurePlayPenBoundaries(Bounds newBounds)
        {
            playPenBoundaries.SetBounds(newBounds);
        }

        // Method to configure world boundaries. You can call this to update boundaries.
        public static void ConfigureWorldBoundaries(Bounds newBounds)
        {
            worldBoundaries.SetBounds(newBounds);
        }


        // Similarly, for plane boundaries.
        public static void ConfigurePlaneBoundaries(Bounds newBounds)
        {
            planeBoundaries.SetBounds(newBounds);
        }
    }
    public abstract class BaseBoundaries2D
    {
        public bool isInitialized = false;
        public Bounds bounds;
        public Vector2 bottomLeft;
        public Vector2 topLeft;
        public Vector2 bottomRight;
        public Vector2 topRight;

        public int cacheSize; // Adjust as needed
        public List<float2> randomPositionsCache;

        // Method to set bounds and update corner points.
        public void SetBounds(Bounds newBounds)
        {
            bounds = newBounds;
            UpdateCornerPoints();

            // Create a cache of random positions within the bounds
            CreateRandomPositionsCache();

            isInitialized = true;

        }

        protected void UpdateCornerPoints()
        {
            bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            topLeft = new Vector2(bounds.min.x, bounds.max.y);
            bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public Vector2 ClampCameraPositionWithinBounds(Vector2 position, float size = 1.0f, float aspect = 1.0f)
        {
            // Calculate halfHeight and halfWidth based on the orthographic size and aspect ratio
            var halfHeight = size / 2.0f; // Half of the orthographic size
            var halfWidth = halfHeight * aspect; // Adjusted for the aspect ratio

            // Calculate the minimum and maximum bounds, taking into account the halfWidth and halfHeight
            var minBounds = new Vector2(bounds.min.x + halfWidth, bounds.min.y + halfHeight);
            var maxBounds = new Vector2(bounds.max.x - halfWidth, bounds.max.y - halfHeight);
            // Clamp the position within these adjusted bounds
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);

            return position;
        }

        public float ClampCameraSize(float size)
        {
            // Calculate min and max sizes based on world boundaries
            // This is an example; adjust the logic to fit your game's needs
            float minSize = 26f; // For example
            float maxSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2f;  // For example

            return Mathf.Clamp(size, minSize, maxSize);
        }
        public Vector3 ClampObjectPositionWithinBounds(GameObject obj)
        {
            Collider2D collider = obj.GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError("Object does not have a Collider2D component.");
                return obj.transform.position; // Return the original position if no collider is found
            }

            Bounds objectBounds = collider.bounds;
            Vector3 objectSize = objectBounds.size;

            // Calculate the clamped position
            float clampedX = Mathf.Clamp(obj.transform.position.x, bounds.min.x + objectSize.x / 2, bounds.max.x - objectSize.x / 2);
            float clampedY = Mathf.Clamp(obj.transform.position.y, bounds.min.y + objectSize.y / 2, bounds.max.y - objectSize.y / 2);

            return new Vector3(clampedX, clampedY, obj.transform.position.z);
        }
        public float2 ClampPosition(float2 position)
        {
            var minBounds = new float2(bounds.min.x, bounds.min.y);
            var maxBounds = new float2(bounds.max.x, bounds.max.y);
            return math.clamp(position, minBounds, maxBounds);
        }

        public void CreateRandomPositionsCache()
        {
            cacheSize = 1000;
            randomPositionsCache = new List<float2>(cacheSize);
            for (int i = 0; i < cacheSize; i++)
            {
                float2 randomPosition = new float2(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
                );
                randomPositionsCache
            .Add(randomPosition);
            }
        }

        public float2 GetRandomPositionFromCache()
        {
            int randomIndex = UnityEngine.Random.Range(0, cacheSize);
            return randomPositionsCache[randomIndex];
        }
    }

    public class WorldBoundaries2D : BaseBoundaries2D
    {
        // Additional specific functionality for world boundaries can be added here.
    }

    public class PlaneBoundaries2D : BaseBoundaries2D
    {
        // Additional specific functionality for plane boundaries.
    }

    public class PlayPenBoundaries2D : BaseBoundaries2D
    {
        // Additional specific functionality for plane boundaries.
    }
}
