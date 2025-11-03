using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CalculateDistance : NetworkBehaviour
{
    [SerializeField] private float _maxDistance;

    [HideInInspector] public List<Transform> connectedPlayers;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        connectedPlayers = new List<Transform>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (IsOwner)
            PlayersManager.Instance.ownerCalculateDistance = this;
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;
        
        Vector3 correctedPosition = transform.position;

        foreach (Transform t in connectedPlayers)
        {
            if (t == null) continue;

            Vector3 direction = correctedPosition - t.position;
            float distance = direction.magnitude;

            if (distance > _maxDistance)
            {
                correctedPosition = t.position + direction.normalized * _maxDistance;
            }
        }

        _rigidbody.MovePosition(correctedPosition);
    }
}
