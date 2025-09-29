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

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        foreach (Transform t in connectedPlayers)
        {
            float distance = Vector3.Distance(transform.position, t.position);
            
            if (distance > _maxDistance)
            {
                if (_rigidbody.linearVelocity.magnitude > .1f)
                {
                    Vector3 directionToOther = (t.position - transform.position).normalized;
                    
                    _rigidbody.position = t.position - directionToOther * _maxDistance;
                }
            }
        }
    }
}
