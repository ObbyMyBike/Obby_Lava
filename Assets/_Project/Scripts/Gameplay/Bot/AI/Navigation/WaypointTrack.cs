using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTrack
{
    private const float DEFAULT_TOLERANCE = 0.001f;
    
    private readonly IReadOnlyList<Transform> points;
    
    private int _index;

    public WaypointTrack(WaypointPath path)
    {
        points = path != null ? path.GetOrderedPoints() : Array.Empty<Transform>();
        _index = 0;
    }
    
    public bool IsFinished => points.Count == 0 || _index >= points.Count;
    public Transform Current => (!IsFinished ? points[_index] : null);

    public void Advance() => _index++;
    
    public void SyncToProgress(float checkpointY, float tolerance = DEFAULT_TOLERANCE)
    {
        if (points.Count == 0)
            return;

        int newIndex = 0;

        for (int i = 0; i < points.Count; i++)
        {
            Transform pointTransform = points[i];
            
            if (pointTransform == null)
                continue;

            if (pointTransform.position.y >= checkpointY - tolerance)
            {
                newIndex = i;
                
                break;
            }
        }

        _index = newIndex;
    }
    
    public bool SyncToNearestInYBand(Vector3 worldPosition, float maxAbove, float maxBelow)
    {
        if (points.Count == 0)
            return false;

        float minPositionY = worldPosition.y - Mathf.Max(0f, maxBelow);
        float maxPositionY = worldPosition.y + Mathf.Max(0f, maxAbove);

        int bestIndex = -1;
        float bestSqr = float.PositiveInfinity;
        Vector2 currentPoint = new Vector2(worldPosition.x, worldPosition.z);

        for (int i = 0; i < points.Count; i++)
        {
            Transform pointTransform = points[i];
            
            if (pointTransform == null)
                continue;

            float pointTransformY = pointTransform.position.y;
            
            if (pointTransformY < minPositionY || pointTransformY > maxPositionY)
                continue;

            Vector2 nextPoint = new Vector2(pointTransform.position.x, pointTransform.position.z);
            float sqr = (currentPoint - nextPoint).sqrMagnitude;
            
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                bestIndex = i;
            }
        }

        if (bestIndex < 0)
            return false;
        
        _index = bestIndex;
        
        return true;
    }
    
    public void SyncToNearest(Vector3 worldPosition)
    {
        if (points.Count == 0) return;

        int bestIndex = 0;
        float bestSqr = float.PositiveInfinity;
        Vector2 currentPoint = new Vector2(worldPosition.x, worldPosition.z);

        for (int i = 0; i < points.Count; i++)
        {
            Transform pointTransform = points[i];
            
            if (pointTransform == null)
                continue;

            Vector2 nextPoint = new Vector2(pointTransform.position.x, pointTransform.position.z);
            float sqr = (currentPoint - nextPoint).sqrMagnitude;
            
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                bestIndex = i;
            }
        }

        _index = bestIndex;
    }
}