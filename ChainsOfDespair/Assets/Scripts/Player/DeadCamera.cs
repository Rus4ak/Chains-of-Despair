using System.Collections.Generic;
using UnityEngine;

public class DeadCamera : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 200f;
    [SerializeField] private float _distance = 10f;
    [SerializeField] private float _minY = -20f;
    [SerializeField] private float _maxY = 80f;
    [SerializeField] private float _heightOffset = 5f;


    private float _currentX = 0;
    private float _currentY = 0;
    private int _targetIndex = 0;
    private List<PlayerInitialize> _players;
    private Transform _target;
    private float _currentDistance;

    private void Start()
    {
        _players = PlayersManager.Instance.players;
        ChangeTarget();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _targetIndex--;

            if (_targetIndex < 0)
                _targetIndex = _players.Count - 1;
        }

        if (Input.GetMouseButtonDown(1))
        {
            _targetIndex++;

            if (_targetIndex > _players.Count - 1)
                _targetIndex = 0;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            ChangeTarget();
        }

        _currentX += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        _currentY -= Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
        _currentY = Mathf.Clamp(_currentY, _minY, _maxY);
    }

    private void ChangeTarget()
    {
        if (_players[_targetIndex].isAlive)
            _target = _players[_targetIndex].transform;
        else
            _target = _players[_targetIndex].ragdoll.transform;

        transform.SetParent(_target);
    }

    private void LateUpdate()
    {
        Vector3 pivot = _target.position + Vector3.up * _heightOffset;

        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        Vector3 desiredPosition = pivot - rotation * Vector3.forward * _distance;

        if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit))
        {
            _currentDistance = Vector3.Distance(pivot, hit.point) - 0.2f;
        }
        else
        {
            _currentDistance = _distance;
        }

        Vector3 finalPosition = pivot - rotation * Vector3.forward * _currentDistance;

        transform.position = finalPosition;
        transform.LookAt(pivot);
    }
}
