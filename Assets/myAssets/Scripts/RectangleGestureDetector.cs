using System.Collections.Generic;
using UnityEngine;

public class RectangleGestureDetector : MonoBehaviour
{
    public bool IsActiveGesture;
    public Material lineMaterial;
    public Color lineColor = new Color(1f, 0f, 0f, 0.88f);

    // Minimum distance a point must move to be considered a new point in the gesture
    public float minMoveDistance = 10f;
    // Tolerance for an angle to be considered a right angle (90 degrees)
    public float angleTolerance = 20f;
    // Tolerance for the start and end points to be considered "closed"
    public float closureTolerance = 50f;
    // Minimum and maximum number of points expected for a rectangle (approx corners)
    public int minCorners = 3;
    public int maxCorners = 5;

    private List<Vector2> gesturePoints = new List<Vector2>();
    private bool isDrawing = false;


    void Update()
    {
        if (!IsActiveGesture) return;
        // Handle mouse input (can be adapted for touch input)
        Vector2 tmp;
        tmp = Input.mousePosition;
        tmp.y = Screen.height - tmp.y;
        if (Input.GetMouseButtonDown(0))
        {
            StartGesture(tmp);
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateGesture(tmp);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndGesture();
        }
    }

    void StartGesture(Vector2 screenPos)
    {
        isDrawing = true;
        gesturePoints.Clear();
        gesturePoints.Add(screenPos);
    }

    void UpdateGesture(Vector2 screenPos)
    {
        if (isDrawing)
        {
            // Only add a new point if the user has moved sufficiently
            if (Vector2.Distance(screenPos, gesturePoints[gesturePoints.Count - 1]) > minMoveDistance)
            {
                gesturePoints.Add(screenPos);
            }
        }
    }

    void EndGesture()
    {
        if (isDrawing)
        {
            isDrawing = false;
            // Ensure the gesture has enough points to be considered a shape
            if (gesturePoints.Count > 10) // Arbitrary minimum point count
            {
                DetectRectangle();
            }
            else
            {
                Debug.Log("Gesture too short, not a rectangle.");
            }
        }
    }

    void DetectRectangle()
    {
        // 1. Check if the shape is closed
        if (Vector2.Distance(gesturePoints[0], gesturePoints[gesturePoints.Count - 1]) > closureTolerance)
        {
            Debug.Log("Gesture not closed.");
            return;
        }

        // A full implementation would involve shape recognition algorithms like the $P Point-Cloud Recognizer.
        // The simple approach below approximates a bounding box.

        // Simple Bounding Box check (not true gesture detection but a common approximation):
        Rect boundingBox = GetBoundingBox(gesturePoints);
        float aspectRatio = boundingBox.width / boundingBox.height;

        // Check if it has a reasonable aspect ratio for a rectangle/square (e.g., between 0.5 and 2.0)
        if (aspectRatio > 0.5f && aspectRatio < 2.0f)
        {
            Debug.Log("Rectangle gesture detected! Bounding Box: " + boundingBox);
            // Add custom event/action here
        }
        else
        {
            Debug.Log("Gesture shape not a rectangle (aspect ratio issues).");
        }
    }

    Rect GetBoundingBox(List<Vector2> points)
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float maxY = Mathf.NegativeInfinity;

        foreach (Vector2 point in points)
        {
            if (point.x < minX) minX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.x > maxX) maxX = point.x;
            if (point.y > maxY) maxY = point.y;
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    // Optional: Draw the recorded gesture in the Scene view
    //void OnDrawGizmos()
    //{
    //    if (!IsActiveGesture) return;
    //    if (gesturePoints != null && gesturePoints.Count > 1)
    //    {
    //        Gizmos.color = Color.cyan;
    //        for (int i = 1; i < gesturePoints.Count - 1; i++)
    //        {
    //            Gizmos.DrawLine(gesturePoints[i - 1], gesturePoints[i]);

    //            // Note: Gizmos work in world space. This simple example is in screen space.
    //            // For a proper 3D/VR implementation, you would track world space positions.
    //        }
    //        Gizmos.DrawLine(gesturePoints[gesturePoints.Count - 1], gesturePoints[0]);
    //    }
    //}

    void OnGUI()
    {
        if (!IsActiveGesture) return;
        DrawList();
    }

    void DrawList()
    {
        if (Event.current.type != EventType.Repaint)
            return;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.Begin(GL.LINES);

        DrawLines();

        GL.End();
        GL.PopMatrix();
    }

    void DrawLines()
    {
        // draw outline
        GL.Color(lineColor);
        for (int i = 1; i < gesturePoints.Count - 1; i++)
        {
            DrawLine(gesturePoints[i - 1], gesturePoints[i]);
        }
        //DrawLine(gesturePoints.Last(), gesturePoints[0]);
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex(p1);
        GL.Vertex(p2);
    }
}
