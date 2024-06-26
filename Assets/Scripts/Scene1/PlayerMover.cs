using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PlayerMover : MonoBehaviour,IPlayerMoveService
{
    private List<GameObject> _wayPoints;
    private int _currentPointIndex;
    private bool _isMoving;
    private float _pathDuration;
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
        if(_isMoving) return;
        
        MoveToPoint(_currentPointIndex < _wayPoints.Count -1 ? _currentPointIndex+1 : 0);
    }

    public void MoveToPreviousPoint()
    {
        if(_isMoving || _currentPointIndex == 0) return;

        MoveToPoint(_currentPointIndex - 1);
    }

    public void MoveToTheEnd()
    {
        if(_isMoving || _currentPointIndex == _wayPoints.Count -1) return;

        MoveToPoint(_wayPoints.Count-1);
    }

    public void MoveToStart()
    {
        if(_isMoving || _currentPointIndex == 0) return;
        
        MoveToPoint(0);
    }
    
    public void MoveToEndOnPath()
    {
        if(_isMoving || _currentPointIndex == _wayPoints.Count -1) return;

        _isMoving = true;
        transform.DOKill();
        var currentPath = _wayPoints.Skip(_currentPointIndex).Select(t => t.transform.position).ToArray();
        _pathDuration = Mathf.Min(Constants.MAX_PATH_DURATION, currentPath.Length * Constants.POINT_MOVE_DURATION); //To avoid too long wait durations
        transform.DOPath(currentPath, _pathDuration).SetTarget(this).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _currentPointIndex = _wayPoints.Count - 1;
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
            });
    }
}