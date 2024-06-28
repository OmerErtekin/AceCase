using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PlayerMover : MonoBehaviour,IPlayerMoveService
{
    #region Variables
    private List<GameObject> _wayPoints;
    private int _currentPointIndex;
    private bool _isMoving;
    private float _pathDuration;
    private readonly Queue<MovementAction> _queuedSteps = new();
    #endregion

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<IPlayerMoveService>(this);
    }

    private void Start()
    {
        _wayPoints = ServiceLocator.Instance.GetService<IWaypointService>().Waypoints;
        transform.position = _wayPoints[0].transform.position;
    }

    public void MoveToNextPoint()
    {
        if (_isMoving)
        {
            _queuedSteps.Enqueue(MovementAction.Forward);
            return;
        }

        MoveToPoint(_currentPointIndex < _wayPoints.Count - 1 ? _currentPointIndex + 1 : 0);
    }

    public void MoveToPreviousPoint()
    {
        if (_isMoving)
        {
            _queuedSteps.Enqueue(MovementAction.Backward);
            return;
        }

        MoveToPoint(_currentPointIndex > 0 ? _currentPointIndex - 1 : _wayPoints.Count - 1);
    }


    public void MoveToStart()
    {
        if(_currentPointIndex == 0) return;

        if (_isMoving)
        {
            _queuedSteps.Enqueue(MovementAction.GoToStart);
            return;
        }
        
        MoveToPoint(0);
    }
    
    public void MoveToEndOnPath()
    {
        if (_currentPointIndex == _wayPoints.Count - 1)  return;

        if (_isMoving)
        {
            _queuedSteps.Enqueue(MovementAction.GoToEnd);
            return;
        }

        MoveOnPath(_currentPointIndex,_wayPoints.Count-1,false);
    }
    
    public void MoveToStartOnPath()
    {
        if(_currentPointIndex == 0) return;

        if (_isMoving)
        {
            _queuedSteps.Enqueue(MovementAction.GoToStartOnpPath);
            return;
        }

        MoveOnPath(_currentPointIndex,0,true);
    }

    private void MoveOnPath(int startIndex, int endIndex,bool isBackwards)
    {
        _isMoving = true;
        transform.DOKill();
        Vector3[] currentPath;
        if (!isBackwards)
        {
            currentPath = _wayPoints.GetRange(startIndex, endIndex - startIndex + 1).Select(t => t.transform.position).ToArray();
        }
        else
        {
            currentPath = _wayPoints.GetRange(endIndex,startIndex - endIndex).Select(t => t.transform.position).Reverse().ToArray();
        }
        
        _pathDuration = Mathf.Min(Constants.MAX_PATH_DURATION, currentPath.Length * Constants.POINT_MOVE_DURATION); //To avoid too long wait durations
        transform.DOPath(currentPath, _pathDuration).SetTarget(this).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _currentPointIndex = endIndex;
                PerformNextQueuedAction();
            });
    }
    
    private void MoveToPoint(int index)
    {
        _isMoving = true;
        transform.DOKill();
        transform.DOMove(_wayPoints[index].transform.position, Constants.POINT_MOVE_DURATION).SetTarget(this).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _currentPointIndex = index;
                PerformNextQueuedAction();
            });
    }

    private void PerformNextQueuedAction()
    {
        if(_queuedSteps.Count == 0) return;

        switch (_queuedSteps.Dequeue())
        {
            case MovementAction.Forward:
                MoveToNextPoint();
                break;
            case MovementAction.Backward:
                MoveToPreviousPoint();
                break;
            case MovementAction.GoToEnd:
                MoveToEndOnPath();
                break;
            case MovementAction.GoToStart:
                MoveToStart();
                break;
            case MovementAction.GoToStartOnpPath:
                MoveToStartOnPath();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}

public enum MovementAction
{
    Forward,
    Backward,
    GoToEnd,
    GoToStart,
    GoToStartOnpPath
}
