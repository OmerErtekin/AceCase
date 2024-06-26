using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{

    [SerializeField] private Button NextWaypointButton;
    [SerializeField] private Button GoToEndButton;
    
    // Start is called before the first frame update
    void Start()
    {
        NextWaypointButton.onClick.AddListener(OnNextWaypoint);
        GoToEndButton.onClick.AddListener(OnGoToEnd);
    }
    
    private void OnNextWaypoint()
    {
        // Fill this method!
        
        Debug.Log("You clicked next waypoint button!");
    }

    private void OnGoToEnd()
    {
        // Fill this method!
        
        Debug.Log("You clicked go to the end button!");
    }

}
