using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [SerializeField] private int _segments = 19;
    [SerializeField] private float _segmentLength = 2f;
    [SerializeField] private int _solveIterations = 20;
    [SerializeField] private GameObject _linkPrefab;

    private AudioSource _audioSource;
    private Transform _start;
    private Transform _end;
    private bool _isInitialized = false;

    private List<Rigidbody> _connectedPlayers;
    private List<Vector3> _points;
    private List<Vector3> _oldPoints;
    private List<GameObject> _links;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(Transform startPoint, Transform endPoint, List<Rigidbody> players)
    {
        _start = startPoint;
        _end = endPoint;

        _connectedPlayers = players;

        _points = new List<Vector3>();
        _oldPoints = new List<Vector3>();
        _links = new List<GameObject>();

        foreach (var p in players)
        {
            p.GetComponent<PlayerInitialize>().chainGO.Add(gameObject);
        }

        for (int i = 0; i < _segments; i++)
        {
            Vector3 p = Vector3.Lerp(_start.position, _end.position, i / (float)(_segments - 1));
            _points.Add(p);
            _oldPoints.Add(p);

            if (i < _segments - 1)
            {
                var link = Instantiate(_linkPrefab, p, Quaternion.identity, transform);
                _links.Add(link);
            }
        }

        _isInitialized = true;
    }

    private void Update()
    {
        if (_connectedPlayers[0].linearVelocity.magnitude > .1f || _connectedPlayers[1].linearVelocity.magnitude > .1f)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (!_isInitialized)
            return;

        Simulate();
        ApplyConstraints();
        UpdateVisuals();
    }

    private void Simulate()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            Vector3 temp = _points[i];
            _points[i] += (_points[i] - _oldPoints[i]);
            _points[i] += Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            _oldPoints[i] = temp;
        }
    }

    private void ApplyConstraints()
    {
        if (_start) _points[0] = _start.position;
        if (_end) _points[_points.Count - 1] = _end.position;

        for (int iter = 0; iter < _solveIterations; iter++)
        {
            for (int i = 0; i < _points.Count - 1; i++)
            {
                Vector3 delta = _points[i + 1] - _points[i];
                float dist = delta.magnitude;
                float error = dist - _segmentLength;
                Vector3 change = delta.normalized * (error * 0.5f);

                if (i != 0) _points[i] += change;
                if (i + 1 != _points.Count - 1) _points[i + 1] -= change;
            }
        }
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < _links.Count; i++)
        {
            Vector3 posA = _points[i];
            Vector3 posB = _points[i + 1];

            Vector3 midPoint = (posA + posB) * 0.5f;
            _links[i].transform.position = midPoint;

            Vector3 dir = (posB - posA).normalized;

            Quaternion rot = Quaternion.LookRotation(dir);

            if (i % 2 == 1)
            {
                rot *= Quaternion.Euler(0, 0, 90f);
            }

            _links[i].transform.rotation = rot;
        }
    }
}
