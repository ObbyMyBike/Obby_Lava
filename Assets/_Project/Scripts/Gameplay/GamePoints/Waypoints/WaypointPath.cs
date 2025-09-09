using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new List<Transform>();

    public IReadOnlyList<Transform> GetOrderedPoints() => _points;
}