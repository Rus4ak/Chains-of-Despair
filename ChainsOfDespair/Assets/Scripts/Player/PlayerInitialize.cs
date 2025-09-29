using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInitialize : NetworkBehaviour
{
    [SerializeField] private GameObject _chain;
    [SerializeField] private Transform _forwardJoint;
    [SerializeField] private Transform _backJoint;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        List<PlayerInitialize> players = PlayersManager.Instance.players;

        if (players.Count > 0)
        {
            GameObject chainGO = Instantiate(_chain);
            Chain chain = chainGO.GetComponent<Chain>();

            List<Rigidbody> playersRb = new List<Rigidbody>()
            {
                GetComponent<Rigidbody>(),
                players[players.Count - 1].GetComponent<Rigidbody>()
            };

            chain.Initialize(players[players.Count - 1]._backJoint, _forwardJoint, playersRb);

            GetComponent<CalculateDistance>().connectedPlayers.Add(players[players.Count - 1].transform);
            players[players.Count - 1].GetComponent<CalculateDistance>().connectedPlayers.Add(transform);
        }

        players.Add(this);

        if (IsOwner)
            PlayersManager.Instance.ownerPlayer = this;
    }

    public void Spawn(Vector3 position)
    {
        if (IsOwner)
        {
            _rigidbody.position = position;
        }
    }
}
