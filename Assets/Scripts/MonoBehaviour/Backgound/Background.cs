using System;
using UnityEditor;
using UnityEngine;

namespace Borgs
{
    public class Background : MonoBehaviour
    {
        void Awake()
        {
            CalculatePlaneBoundaries();
        }

        void OnDrawGizmos()
        {
            CalculatePlaneBoundaries();


            PlaneBoundaries2D planeBoundaries = Globals.planeBoundaries;

            // Draw lines between corners
            Gizmos.color = Color.green;

            Gizmos.DrawLine(planeBoundaries.bottomLeft, planeBoundaries.topLeft); // leftside-upwards
            Gizmos.DrawLine(planeBoundaries.topLeft, planeBoundaries.topRight); // topline-rightwards
            Gizmos.DrawLine(planeBoundaries.topRight, planeBoundaries.bottomRight); // rightside-downwards
            Gizmos.DrawLine(planeBoundaries.bottomRight, planeBoundaries.bottomLeft); // bottomline-leftwards

            // Draw text labels for side lengths
            DrawText("BottomLeft", planeBoundaries.bottomLeft, Vector3.left);
            DrawText("TopLeft", planeBoundaries.topLeft, Vector3.up);
            DrawText("TopRight", planeBoundaries.topRight, Vector3.right);
            DrawText("BottomRight", planeBoundaries.bottomRight, Vector3.down);

            // Draw text labels halfway

            Vector2 halfWayLeft = new Vector2(planeBoundaries.bottomLeft.x, planeBoundaries.bottomLeft.y) * new Vector2(1.0f, 0);
            Vector2 halfWayTop = new Vector2(planeBoundaries.topLeft.x, planeBoundaries.topLeft.y) * new Vector2(0, 1.0f);
            Vector2 halfWayRight = new Vector2(planeBoundaries.bottomLeft.x, planeBoundaries.bottomLeft.y) * new Vector2(-1.0f, 0);
            Vector2 halfWayBottom = new Vector2(planeBoundaries.bottomLeft.x, planeBoundaries.bottomLeft.y) * new Vector2(0, 1.0f);

            DrawText("Left: " + Vector2.Distance(planeBoundaries.bottomLeft, planeBoundaries.topLeft).ToString("F2"), halfWayLeft, Vector3.left);
            DrawText("Top: " + Vector2.Distance(planeBoundaries.topLeft, planeBoundaries.topRight).ToString("F2"), halfWayTop, Vector3.up);
            DrawText("Right: " + Vector2.Distance(planeBoundaries.topRight, planeBoundaries.bottomRight).ToString("F2"), halfWayRight, Vector3.right);
            DrawText("Bottom: " + Vector2.Distance(planeBoundaries.bottomRight, planeBoundaries.bottomLeft).ToString("F2"), halfWayBottom, Vector3.down);

        }

        void DrawText(string text, Vector3 position, Vector3 offset)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            Handles.Label(position + offset, text, style);
        }

        void CalculatePlaneBoundaries()
        {
            if (GetComponent<MeshRenderer>() != null)
            {
                Gizmos.color = Color.green; // Color for background boundary

                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                Vector3 size = meshRenderer.bounds.size;
                Quaternion rotation = transform.rotation;
                Vector3 center = meshRenderer.bounds.center;

                // Define corners in local space relative to the unrotated plane
                Vector3 topRightLocal = new Vector3(-size.x / 2, 0, -size.y / 2);
                Vector3 topLeftLocal = new Vector3(size.x / 2, 0, -size.y / 2);

                Vector3 bottomRightLocal = new Vector3(-size.x / 2, 0, size.y / 2);
                Vector3 bottomLeftLocal = new Vector3(size.x / 2, 0, size.y / 2);

                Bounds newBounds = new Bounds(center, meshRenderer.bounds.size);
                Globals.planeBoundaries.SetBounds(newBounds);

                // Rotate the corners according to the plane's rotation and then translate to world space
                Globals.planeBoundaries.bottomLeft = center + rotation * bottomLeftLocal;
                Globals.planeBoundaries.topLeft = center + rotation * topLeftLocal;
                Globals.planeBoundaries.bottomRight = center + rotation * bottomRightLocal;
                Globals.planeBoundaries.topRight = center + rotation * topRightLocal;

            }
        }


    }
}