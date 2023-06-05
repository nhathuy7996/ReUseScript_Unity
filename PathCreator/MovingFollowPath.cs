using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingFollowPath : MonoBehaviour
{
    PathCreator _path;
    [SerializeField] bool _autoGetPathInParent = false;
    [SerializeField] MoveType _moveType = MoveType.MoveTowards;
    [SerializeField] bool _isMoveBackWard = false;
    [SerializeField] float _speedMoving;

    int _currentIndex = 0;
    int _moveDir = 1;
    List<Vector3> _points => _path.getPoints();
    Action<int> _callBackOnPointReach;
    // Start is called before the first frame update
    void Start()
    {
        if (!_path)
        {
            if (_autoGetPathInParent)
                _path = this.GetComponentInParent<PathCreator>();
            else
                _path = FindObjectOfType<PathCreator>();
        }

        if (!_path)
        {
            Debug.LogError("This component need path creator for follow!");
        }
    }

    public MovingFollowPath Init(Action<int> callBackOnPointReach = null)
    {
        _currentIndex = 0;
        _moveDir = 1;

        this._callBackOnPointReach = callBackOnPointReach;
        return this;
    }

    public MovingFollowPath SetMoveType(MoveType moveType)
    {
        this._moveType = moveType;
        return this;
    }

    public MovingFollowPath setSpeed(float speed)
    {
        this._speedMoving = speed;
        return this;
    }



    // Update is called once per frame
    void Update()
    {
        if (!_path)
        {
            Debug.LogError("This component need path creator for follow!");
            return;
        }

        switch (_moveType)
        {
            case MoveType.MoveTowards:
                this.transform.position = Vector3.MoveTowards(this.transform.position, _points[_currentIndex], _speedMoving * Time.deltaTime);
                break;
            case MoveType.Lerp:
                this.transform.position = Vector3.Lerp(this.transform.position, _points[_currentIndex], _speedMoving * Time.deltaTime);
                break;
            case MoveType.SLerp:
                this.transform.position = Vector3.Slerp(this.transform.position, _points[_currentIndex], _speedMoving * Time.deltaTime);
                break;
        }

        if (Vector2.Distance(this.transform.position, _points[_currentIndex]) < 0.5f)
        {
            this._callBackOnPointReach?.Invoke(_currentIndex);
            _currentIndex += _moveDir;
            
        }

        if (!_isMoveBackWard)
        {
            if (_currentIndex >= _points.Count)
            {
                _currentIndex = 0;
                return;
            }
        }

        if (_currentIndex >= _points.Count && _moveDir == 1)
        {
            _moveDir = -1;
            _currentIndex = _points.Count -2;
            return;
        }

        if (_currentIndex <= 0 && _moveDir == -1)
        {
            _moveDir = 1;
            _currentIndex = 0;
            return;
        }
    }
}

public enum MoveType
{
    MoveTowards,
    Lerp,
    SLerp
}
