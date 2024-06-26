using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PlayerMover : MonoBehaviour,IPlayerMoveService
{
    [SerializeField] private float _moveDuration;
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
        var path = _wayPoints.Skip(_currentPointIndex).Select(t => t.transform.position).ToArray();
        _pathDuration = Mathf.Min(2, path.Length * _moveDuration); //To avoid too long wait durations
        transform.DOPath(path, _pathDuration).SetTarget(this).SetEase(Ease.Linear)
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
        transform.DOMove(_wayPoints[index].transform.position, _moveDuration).SetTarget(this).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isMoving = false;
                _currentPointIndex = index;
            });
    }
}