using UnityEngine;
using System.Collections.Generic;

public class WaypointPathCreator : MonoBehaviour,IWaypointService
{
    #region Variables
    [SerializeField] private GameObject _waypointPrefab;
    public List<GameObject> Waypoints { get; private set; }
    #endregion
    
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<IWaypointService>(this);
        Waypoints = new List<GameObject>();
        
        // Initialize path of Waypoints
        List<Vector2> path = GeneratePath();
        foreach(Vector2 waypoint in path)
        {
            GameObject waypointObj = Instantiate(_waypointPrefab, new Vector3(waypoint.x, waypoint.y, -25f), _waypointPrefab.transform.rotation, transform);
            Waypoints.Add(waypointObj);
        }
    }

    private static List<Vector2> GeneratePath()
    {
        // Span around origin
        int span = 10;

        // Choose amount of turn points
        int amountOfTurnPoints = Random.Range(span, span);
        List<Vector2> turnPoints = new List<Vector2>();

        // Generate positions for turn points
        for (int i = 0; i < amountOfTurnPoints; i++)
        {
            turnPoints.Add(new Vector2(Random.Range((i*2)-span, (i*2)+2-span), Random.Range(-span, span+1)));
        }

        // Sort turn points according to x
        turnPoints.Sort((v1, v2) => v1.x.CompareTo(v2.x));

        // Add start and end turn points
        Vector2 startPoint = new Vector2(-span, turnPoints[0].y);
        Vector2 endPoint = new Vector2(span, turnPoints[turnPoints.Count - 1].y);
        turnPoints.Insert(0, startPoint);
        turnPoints.Add(endPoint);

        // Setup starting point
        int x = (int)startPoint.x;
        int y = (int)startPoint.y;

        // Build path of vectors
        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < turnPoints.Count; i++)
        {
            while (x != (int)turnPoints[i].x)
            {
                path.Add(new Vector2(x, y));
                x++;
            }
            while (y != (int)turnPoints[i].y)
            {
                path.Add(new Vector2(x, y));
                int dist = ((int)turnPoints[i].y - y);
                y += dist / Mathf.Abs(dist);
            }
        }

        // Add last path vector
        path.Add(new Vector2(x, y));
        return path;
    }
}