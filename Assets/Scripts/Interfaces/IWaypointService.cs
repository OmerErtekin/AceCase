using System.Collections.Generic;
using UnityEngine;

public interface IWaypointService
{
    public List<GameObject> Waypoints { get; }
}