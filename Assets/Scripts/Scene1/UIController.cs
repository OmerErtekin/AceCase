using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Components
    [SerializeField] private Button NextWaypointButton;
    [SerializeField] private Button GoToEndButton;
    [SerializeField] private Button GoToEndOnPathButton;
    [SerializeField] private Button GoToPreviousButton;
    [SerializeField] private Button GoToStartButton;
    private IPlayerMoveService _moveService;
    #endregion

    private void Start()
    {
        NextWaypointButton.onClick.AddListener(OnNextWaypoint);
        GoToEndButton.onClick.AddListener(OnGoToEnd);
        GoToEndOnPathButton.onClick.AddListener(OnGoToEndOnPath);
        GoToPreviousButton.onClick.AddListener(OnGoToPreviousPoint);
        GoToStartButton.onClick.AddListener(OnGoToStart);
        _moveService = ServiceLocator.Instance.GetService<IPlayerMoveService>();
    }
    
    private void OnNextWaypoint()
    {
        _moveService.MoveToNextPoint();
        Debug.Log("You clicked next waypoint button!");
    }

    private void OnGoToEnd()
    {
        _moveService.MoveToTheEnd();
        Debug.Log("You clicked go to the end button!");
    }
    
    private void OnGoToEndOnPath()
    {
        _moveService.MoveToEndOnPath();
        Debug.Log("You clicked go to the end on path button!");
    }
    
    private void OnGoToPreviousPoint()
    {
        _moveService.MoveToPreviousPoint();
        Debug.Log("You clicked go to the previous point button!");
    }
    
    private void OnGoToStart()
    {
        _moveService.MoveToStart();
        Debug.Log("You clicked go to start point button!");
    }
}
